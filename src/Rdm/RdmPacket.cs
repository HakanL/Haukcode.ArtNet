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
