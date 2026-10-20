namespace Haukcode.ArtNet.Rdm;

public abstract class RdmPacket
{
    protected RdmPacket()
    {
    }

    public RdmPacket(RdmCommands command, RdmParameters parameterId)
    {
        Header = new RdmHeader();
        Header.Command = command;
        Header.ParameterId = parameterId;
    }

    #region Contents

    public RdmHeader Header { get; protected set; } = null!;

    public ushort Checksum { get; set; }

    #endregion

    #region Read and Write

    protected void ReadHeader(RdmBinaryReader data)
    {
        Header.ReadData(data);
    }

    protected void WriteHeader(RdmBinaryWriter data)
    {
        Header.WriteData(data);
    }

    protected abstract void ReadData(RdmBinaryReader data);

    protected abstract void WriteData(RdmBinaryWriter data);

    /// <summary>
    /// Reads a packet. When the 2 checksum bytes follow the message they are read into
    /// <see cref="Checksum"/>, but not checked; use <see cref="ReadPacket(ReadOnlySpan{byte}, bool)"/>
    /// to check them.
    /// </summary>
    public static RdmPacket ReadPacket(RdmBinaryReader data)
    {
        RdmHeader header = new RdmHeader();
        header.ReadData(data);

        var rdmPacket = ReadBody(header, ReadParameterData(header, data), typed: true);

        if (data.BytesLeft >= 2)
            rdmPacket.Checksum = data.ReadUInt16();

        return rdmPacket;
    }

    /// <summary>
    /// Exactly the PDL bytes of parameter data, so a packet can't read into the checksum or the
    /// next message.
    /// </summary>
    private static byte[] ReadParameterData(RdmHeader header, RdmBinaryReader data)
    {
        var parameterData = data.ReadBytes(header.ParameterDataLength);
        if (parameterData.Length < header.ParameterDataLength)
            throw new InvalidPacketException($"The message has {parameterData.Length} bytes of parameter data but its PDL is {header.ParameterDataLength}.");

        return parameterData;
    }

    /// <summary>
    /// Parses parameter data as the packet type registered for the header (or
    /// <see cref="RdmRawPacket"/>), reading it from its own buffer.
    /// </summary>
    private static RdmPacket ReadBody(RdmHeader header, byte[] parameterData, bool typed)
    {
        RdmPacket? rdmPacket = (typed ? RdmPacket.Create(header) : null) ?? RdmPacket.Create(header, typeof(RdmRawPacket));
        if (rdmPacket == null)
            throw new UnknownRdmPacketException(header);

        using var stream = new MemoryStream(parameterData);
        try
        {
            rdmPacket.ReadData(new RdmBinaryReader(stream));
        }
        catch (EndOfStreamException)
        {
            throw new InvalidPacketException($"The {header.ParameterDataLength}-byte parameter data is too short for {rdmPacket.GetType().FullName}.");
        }

        return rdmPacket;
    }

    /// <summary>
    /// Reads a response that was split with ACK_OVERFLOW: the parameter data of every message, in
    /// order, is joined and parsed as one packet. Each message is RDM data as ArtRdm carries it
    /// (from the Message Length byte on, ending with the checksum); every message but the last
    /// has response type ACK_OVERFLOW.
    /// </summary>
    /// <exception cref="InvalidPacketException">A checksum is wrong, or the messages aren't one overflow sequence.</exception>
    public static RdmPacket ReadOverflowPacket(IEnumerable<ReadOnlyMemory<byte>> rdmMessages, bool validateChecksum = true)
    {
        RdmHeader? header = null;
        using var parameterData = new MemoryStream();

        foreach (var message in rdmMessages)
        {
            if (header != null && (RdmResponseTypes)header.PortOrResponseType != RdmResponseTypes.AckOverflow)
                throw new InvalidPacketException("Only the last message of an ACK_OVERFLOW sequence may have another response type.");

            if (validateChecksum && !IsChecksumValid(message.Span))
                throw new InvalidPacketException("The RDM checksum is missing or doesn't match the message.");

            using var stream = new MemoryStream(message.ToArray());
            var reader = new RdmBinaryReader(stream);

            var messageHeader = new RdmHeader();
            messageHeader.ReadData(reader);

            if (header != null && (messageHeader.ParameterId != header.ParameterId || messageHeader.Command != header.Command))
                throw new InvalidPacketException("The messages are for different parameters or command classes.");

            parameterData.Write(ReadParameterData(messageHeader, reader));
            header = messageHeader;
        }

        if (header == null)
            throw new InvalidPacketException("No messages.");

        header.ParameterDataLength = (int)parameterData.Length;

        return ReadBody(header, parameterData.ToArray(), typed: true);
    }

    /// <summary>
    /// Reads a packet from RDM data as ArtRdm carries it: from the Message Length byte on, without
    /// the start code, ending with the checksum.
    /// </summary>
    /// <exception cref="InvalidPacketException">The checksum is missing or wrong; E1.20 requires the message to be discarded.</exception>
    public static RdmPacket ReadPacket(ReadOnlySpan<byte> rdmData, bool validateChecksum = true)
    {
        if (validateChecksum && !IsChecksumValid(rdmData))
            throw new InvalidPacketException("The RDM checksum is missing or doesn't match the message.");

        using var stream = new MemoryStream(rdmData.ToArray());
        return ReadPacket(new RdmBinaryReader(stream));
    }

    /// <summary>
    /// True when <paramref name="rdmData"/> (from the Message Length byte on, as in ArtRdm) is
    /// followed by a matching checksum. The checksum also covers the start code and sub-start
    /// code, which aren't in the data.
    /// </summary>
    public static bool IsChecksumValid(ReadOnlySpan<byte> rdmData)
    {
        if (rdmData.IsEmpty)
            return false;

        // Message Length counts the start code and sub-start code but not the checksum; its range
        // is 24 to 255
        if (rdmData[0] < 24)
            return false;

        int covered = rdmData[0] - 2;
        if (rdmData.Length < covered + 2)
            return false;

        ushort sum = (ushort)((int)DmxStartCodes.RDM + (int)RdmVersions.SubMessage);
        foreach (byte value in rdmData[..covered])
            sum += value;

        return sum == (ushort)((rdmData[covered] << 8) | rdmData[covered + 1]);
    }

    public static RdmPacket ReadPacket(RdmCommands command, RdmParameters parameterId, RdmBinaryReader contentData)
    {
        RdmHeader header = new RdmHeader();
        header.Command = command;
        header.ParameterId = parameterId;

        return ReadPacket(header, contentData);
    }

    /// <summary>
    /// Reads a packet body using an already prepared header, for example a deferred/queued
    /// response where <see cref="RdmHeader.PortOrResponseType"/> (ACK_TIMER, NACK_REASON,
    /// ACK_OVERFLOW) must be preserved. Set <see cref="RdmHeader.ParameterDataLength"/> to the
    /// body length, variable-length bodies are read based on it.
    /// </summary>
    public static RdmPacket ReadPacket(RdmHeader header, RdmBinaryReader contentData)
    {
        return ReadBody(header, ReadParameterData(header, contentData), typed: true);
    }

    /// <summary>
    /// Reads a packet when a type is registered for its command class and parameter; false
    /// (and no packet) otherwise.
    /// </summary>
    public static bool TryReadPacket(RdmBinaryReader data, out RdmPacket? rdmPacket)
    {
        RdmHeader header = new RdmHeader();
        header.ReadData(data);

        var parameterData = ReadParameterData(header, data);

        if (RdmPacket.Create(header) == null)
        {
            rdmPacket = null;
            return false;
        }

        rdmPacket = ReadBody(header, parameterData, typed: true);

        if (data.BytesLeft >= 2)
            rdmPacket.Checksum = data.ReadUInt16();

        return true;
    }

    public static RdmRawPacket? ReadPacketRaw(RdmBinaryReader data)
    {
        RdmHeader header = new RdmHeader();
        header.ReadData(data);

        var rdmPacket = (RdmRawPacket)ReadBody(header, ReadParameterData(header, data), typed: false);

        if (data.BytesLeft >= 2)
            rdmPacket.Checksum = data.ReadUInt16();

        return rdmPacket;
    }

    public static void WritePacket(RdmPacket packet, RdmBinaryWriter data)
    {
        WritePacket(packet, data,false);      
    }

    public static void WritePacket(RdmPacket packet, RdmBinaryWriter data, bool onlyContent)
    {
        if (!onlyContent)
            packet.WriteHeader(data);

        packet.WriteData(data);

        if (!onlyContent)
            packet.Header.WriteLength(data);
    }

    public static ushort CalculateChecksum(byte[] data)
    {
        ushort checksum = 0;
        foreach (byte item in data)
            checksum += item;
        return checksum;
    }

    public static ushort CalculateChecksum(byte[] data, int startIndex, int endIndex)
    {
        ushort checksum = 0;
        for (int n=startIndex;n<endIndex;n++)
            checksum += data[n];
        return checksum;
    }

    /// <summary>
    /// The checksum of an RDM message given as ArtRdm carries it (from the Message Length byte
    /// on, without the checksum): the sum of every byte including the start code and sub-start
    /// code, which aren't in the data.
    /// </summary>
    public static ushort CalculateMessageChecksum(ReadOnlySpan<byte> rdmData)
    {
        ushort checksum = (ushort)((int)DmxStartCodes.RDM + (int)RdmVersions.SubMessage);
        foreach (byte item in rdmData)
            checksum += item;
        return checksum;
    }

    /// <summary>
    /// Writes a response as one RDM message, or as an ACK_OVERFLOW sequence when its parameter
    /// data doesn't fit in one (E1.20 6.3.2). Each message is RDM data as ArtRdm carries it,
    /// from the Message Length byte on, ending with the checksum; send them one at a time, each
    /// in reply to the controller repeating its request.
    /// </summary>
    /// <param name="recordSize">
    /// Size of the repeating structure in the parameter data, which the spec doesn't allow to be
    /// split across messages (6 for a list of UIDs, 2 for a list of PIDs); 1 for unstructured data.
    /// </param>
    /// <param name="leadingLength">
    /// Bytes of fixed fields before the repeating structure. They go in the first message only;
    /// "when the PD contains header data, it shall not be repeated in any subsequent overflow packets".
    /// </param>
    /// <remarks>
    /// The header (destination, source, transaction number, message count, sub-device, command
    /// and PID) is copied to every message. Every message but the last has response type
    /// ACK_OVERFLOW; the last has the packet's own response type, ACK unless it was set to
    /// something else (an <see cref="Packets.Management.RdmAckTimer"/> is written as one ACK_TIMER message). A real responder answers each repeated request with that
    /// request's transaction number; set it on each message's header bytes (offset 13) or
    /// rewrite the packet's <see cref="RdmHeader.TransactionNumber"/> and write it again.
    /// </remarks>
    public static IReadOnlyList<byte[]> WriteResponseMessages(RdmPacket packet, int recordSize = 1, int leadingLength = 0)
    {
        if (recordSize < 1 || recordSize > RdmHeader.MaxParameterDataLength)
            throw new ArgumentOutOfRangeException(nameof(recordSize), recordSize, $"The record size must be 1 to {RdmHeader.MaxParameterDataLength}.");
        if (leadingLength < 0 || leadingLength + recordSize > RdmHeader.MaxParameterDataLength)
            throw new ArgumentOutOfRangeException(nameof(leadingLength), leadingLength, "The leading fields and one record must fit in one message.");

        byte[] parameterData;
        using (var stream = new MemoryStream())
        {
            WritePacket(packet, new RdmBinaryWriter(stream), onlyContent: true);
            parameterData = stream.ToArray();
        }

        if (parameterData.Length > leadingLength && (parameterData.Length - leadingLength) % recordSize != 0)
            throw new InvalidOperationException($"The {parameterData.Length - leadingLength} bytes after the leading fields aren't a whole number of {recordSize}-byte records.");

        var messages = new List<byte[]>();
        int offset = 0;
        do
        {
            int capacity = RdmHeader.MaxParameterDataLength - (offset == 0 ? leadingLength : 0);
            int chunk = parameterData.Length - offset <= RdmHeader.MaxParameterDataLength
                ? parameterData.Length - offset
                : (offset == 0 ? leadingLength : 0) + capacity / recordSize * recordSize;

            bool last = offset + chunk == parameterData.Length;
            messages.Add(WriteMessage(packet.Header, last ? (RdmResponseTypes)packet.Header.PortOrResponseType : RdmResponseTypes.AckOverflow, parameterData.AsSpan(offset, chunk)));
            offset += chunk;
        }
        while (offset < parameterData.Length);

        return messages;
    }

    private static byte[] WriteMessage(RdmHeader header, RdmResponseTypes responseType, ReadOnlySpan<byte> parameterData)
    {
        // Message Length counts the start code and sub-start code, which aren't in the data, but
        // not the checksum
        int messageLength = 24 + parameterData.Length;
        var message = new byte[messageLength];

        using (var stream = new MemoryStream(message))
        {
            var writer = new RdmBinaryWriter(stream);
            writer.WriteByte((byte)messageLength);
            writer.WriteUid(header.DestinationId);
            writer.WriteUid(header.SourceId);
            writer.WriteByte(header.TransactionNumber);
            writer.WriteByte((byte)responseType);
            writer.WriteByte(header.MessageCount);
            writer.WriteUInt16(header.SubDevice);
            writer.WriteByte((byte)header.Command);
            writer.WriteUInt16((ushort)header.ParameterId);
            writer.WriteByte((byte)parameterData.Length);
            writer.WriteBytes(parameterData.ToArray());
            writer.WriteUInt16(CalculateMessageChecksum(message.AsSpan(0, messageLength - 2)));
        }

        return message;
    }

    public bool IsOverflow()
    {
        return (RdmResponseTypes) Header.PortOrResponseType == RdmResponseTypes.AckOverflow && (Header.Command == RdmCommands.GetResponse || Header.Command == RdmCommands.SetResponse);
    }

    #endregion

    public static RdmPacket? Create(RdmHeader header)
    {
        return RdmPacketFactory.Build(header);
    }

    public static RdmPacket Create(RdmHeader header, Type packetType)
    {
        RdmPacket packet = (RdmPacket)Activator.CreateInstance(packetType)!;
        packet.Header = header;
        return packet;
    }
}
