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

    public RdmHeader Header { get; protected set; }

    public short Checksum { get; set; }

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
        RdmPacket rdmPacket = null;

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
            rdmPacket = RdmPacket.Create(header, typeof(RdmRawPacket)) as RdmRawPacket;
            if (rdmPacket != null)
            {
                rdmPacket.ReadData(data);
                return rdmPacket;
            }
        }

        throw new UnknownRdmPacketException(header);            
    }

    public static RdmPacket ReadPacket(RdmCommands command, RdmParameters parameterId, RdmBinaryReader contentData)
    {
        RdmPacket rdmPacket = null;

        RdmHeader header = new RdmHeader();
        header.Command = command;
        header.ParameterId = parameterId;

        rdmPacket = RdmPacket.Create(header);
        if (rdmPacket != null)
        {
            rdmPacket.ReadData(contentData);
            return rdmPacket;
        }
        else
        {
            rdmPacket = RdmPacket.Create(header, typeof(RdmRawPacket)) as RdmRawPacket;
            if (rdmPacket != null)
            {
                rdmPacket.ReadData(contentData);
                return rdmPacket;
            }
        }

        throw new UnknownRdmPacketException(header);
    }

    public static bool TryReadPacket(RdmBinaryReader data,out RdmPacket rdmPacket)
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

    public static RdmRawPacket ReadPacketRaw(RdmBinaryReader data)
    {
        RdmHeader header = new RdmHeader();
        header.ReadData(data);

        RdmRawPacket rdmPacket = RdmPacket.Create(header, typeof(RdmRawPacket)) as RdmRawPacket;
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

    public static RdmPacket Create(RdmHeader header)
    {
        return RdmPacketFactory.Build(header);
    }

    public static RdmPacket Create(RdmHeader header, Type packetType)
    {
        RdmPacket packet = (RdmPacket)Activator.CreateInstance(packetType);
        packet.Header = header;
        return packet;
    }
}
