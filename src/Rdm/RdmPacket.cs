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

    public static RdmPacket ReadPacket(RdmBinaryReader data)
    {
        RdmPacket? rdmPacket = null;

        RdmHeader header = new RdmHeader();
        header.ReadData(data);

        rdmPacket = RdmPacket.Create(header);
        if (rdmPacket != null)
        {
            rdmPacket.ReadData(data);
            return rdmPacket;
         }
        else
        {
            RdmRawPacket? rawPacket = RdmPacket.Create(header, typeof(RdmRawPacket)) as RdmRawPacket;
            if (rawPacket != null)
            {
                rawPacket.ReadData(data);
                return rawPacket;
            }
        }

        throw new UnknownRdmPacketException(header);
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
        RdmPacket? rdmPacket = RdmPacket.Create(header);
        if (rdmPacket != null)
        {
            rdmPacket.ReadData(contentData);
            return rdmPacket;
        }
        else
        {
            RdmRawPacket? rawPacket2 = RdmPacket.Create(header, typeof(RdmRawPacket)) as RdmRawPacket;
            if (rawPacket2 != null)
            {
                rawPacket2.ReadData(contentData);
                return rawPacket2;
            }
        }

        throw new UnknownRdmPacketException(header);
    }

    public static bool TryReadPacket(RdmBinaryReader data, out RdmPacket? rdmPacket)
    {
        RdmHeader header = new RdmHeader();
        header.ReadData(data);

        rdmPacket = RdmPacket.Create(header);
        if (rdmPacket != null)
        {
            rdmPacket.ReadData(data);
            return true;
        }

        return false;
    }

    public static RdmRawPacket? ReadPacketRaw(RdmBinaryReader data)
    {
        RdmHeader header = new RdmHeader();
        header.ReadData(data);

        RdmRawPacket? rdmPacket = RdmPacket.Create(header, typeof(RdmRawPacket)) as RdmRawPacket;
        if (rdmPacket != null)
            rdmPacket.ReadData(data);

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
