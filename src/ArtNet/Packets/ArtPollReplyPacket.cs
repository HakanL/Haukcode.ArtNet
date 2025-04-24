namespace Haukcode.ArtNet.Packets;

public class ArtPollReplyPacket : ArtNetPacket
{
    public ArtPollReplyPacket()
        : base(ArtNetOpCodes.PollReply)
    {
    }

    private byte[] ipAddress = new byte[4];

    public byte[] IpAddress
    {
        get { return ipAddress; }
        set
        {
            if (value.Length != 4)
                throw new ArgumentException("The IP address must be an array of 4 bytes.");

            ipAddress = value;
        }
    }

    private byte[] respUID = new byte[6];

    public byte[] RespUID
    {
        get { return respUID; }
        set
        {
            if (value.Length != 6)
                throw new ArgumentException("The Responder Default UID must be an array of 6 bytes.");

            respUID = value;
        }
    }

    public short Port { get; set; } = ArtNetClient.DefaultPort;

    public short FirmwareVersion { get; set; }

    public byte NetSwitch { get; set; }
    
    public byte SubSwitch { get; set; }
    
    public short Oem { get; set; } = 0xff;

    public byte UbeaVersion { get; set; }

    public byte Status { get; set; }

    public short EstaCode { get; set; }

    public string ShortName { get; set; } = string.Empty;

    public string LongName { get; set; } = string.Empty;

    public string NodeReport { get; set; } = string.Empty;

    public short PortCount { get; set; }

    private byte[] portTypes = new byte[4];

    public byte[] PortTypes
    {
        get { return portTypes; }
        set
        {
            if (value.Length != 4)
                throw new ArgumentException("The port types must be an array of 4 bytes.");

            portTypes = value;
        }
    }

    private byte[] goodInput = new byte[4];

    public byte[] GoodInput
    {
        get { return goodInput; }
        set
        {
            if (value.Length != 4)
                throw new ArgumentException("The good input must be an array of 4 bytes.");

            goodInput = value;
        }
    }

    private byte[] _goodOutputA = new byte[4];

    public byte[] GoodOutputA
    {
        get { return _goodOutputA; }
        set
        {
            if (value.Length != 4)
                throw new ArgumentException("The good output must be an array of 4 bytes.");

            _goodOutputA = value;
        }
    }

    private byte[] _goodOutputB = new byte[4];

    public byte[] GoodOutputB
    {
        get { return _goodOutputB; }
        set
        {
            if (value.Length != 4)
                throw new ArgumentException("The good output must be an array of 4 bytes.");

            _goodOutputB = value;
        }
    }

    public byte[] SwIn { get; set; } = new byte[4];

    public byte[] SwOut { get; set; } = new byte[4];

    public byte AcnPriority { get; set; }

    public byte SwMacro { get; set; }

    public byte SwRemote { get; set; }

    public byte Style { get; set; }

    private byte[] macAddress = new byte[6];

    public byte[] MacAddress
    {
        get { return macAddress; }
        set
        {
            if (value.Length != 6)
                throw new ArgumentException("The mac address must be an array of 6 bytes.");

            macAddress = value;
        }
    }

    private byte[] bindIpAddress = new byte[4];

    public byte[] BindIpAddress
    {
        get { return bindIpAddress; }
        set
        {
            if (value.Length != 4)
                throw new ArgumentException("The bind IP address must be an array of 4 bytes.");

            bindIpAddress = value;
        }
    }

    public byte BindIndex { get; set; }

    public byte Status2 { get; set; }

    public byte Status3 { get; set; }

    protected override int DataLength => 229;

    /// <summary>
    /// Interprets the universe address to ensure compatibility with ArtNet I, II and III devices.
    /// </summary>
    /// <param name="outPorts">Whether to get the address for in or out ports.</param>
    /// <param name="portIndex">The port index to obtain the universe for.</param>
    /// <returns>The 15 Bit universe address</returns>
    public int UniverseAddress(bool outPorts, int portIndex)
    {
        int universe;

        if (NetSwitch > 0 || SubSwitch > 0)
        {
            universe = (NetSwitch & 0x7F00);
            universe += (NetSwitch & 0xFF) << 8;

            universe += (SubSwitch & 0x7F00);
            universe += (SubSwitch & 0x0F) << 4;

            universe += (outPorts ? SwOut[portIndex] : SwIn[portIndex]) & 0xF;
        }
        else
        {
            universe = (outPorts ? SwOut[portIndex] : SwIn[portIndex]);
        }

        return universe;
    }

    internal static ArtPollReplyPacket Parse(BigEndianBinaryReader reader)
    {
        var target = new ArtPollReplyPacket();

        target.IpAddress = reader.ReadBytes(4);
        target.Port = reader.ReadInt16Reverse();
        target.FirmwareVersion = reader.ReadInt16();
        target.NetSwitch = reader.ReadByte();
        target.SubSwitch = reader.ReadByte();
        target.Oem = reader.ReadInt16();
        target.UbeaVersion = reader.ReadByte();
        target.Status = reader.ReadByte();
        target.EstaCode = reader.ReadInt16Reverse();
        target.ShortName = reader.ReadString(18);
        target.LongName = reader.ReadString(64);
        target.NodeReport = reader.ReadString(64);
        target.PortCount = reader.ReadInt16();
        target.PortTypes = reader.ReadBytes(4);
        target.GoodInput = reader.ReadBytes(4);
        target.GoodOutputA = reader.ReadBytes(4);
        target.SwIn = reader.ReadBytes(4);
        target.SwOut = reader.ReadBytes(4);
        target.AcnPriority = reader.ReadByte();
        target.SwMacro = reader.ReadByte();
        target.SwRemote = reader.ReadByte();
        
        reader.SkipBytes(3);
        
        target.Style = reader.ReadByte();
        target.MacAddress = reader.ReadBytes(6);
        target.BindIpAddress = reader.ReadBytes(4);
        target.BindIndex = reader.ReadByte();
        target.Status2 = reader.ReadByte();
        target.GoodOutputB = reader.ReadBytes(4);
        target.Status3 = reader.ReadByte();
        target.RespUID = reader.ReadBytes(6);

        reader.SkipBytes(15);

        return target;
    }

    protected override void WriteData(BigEndianBinaryWriter writer)
    {
        writer.WriteBytes(IpAddress);
        writer.WriteInt16Reverse(Port);
        writer.WriteInt16(FirmwareVersion);
        writer.WriteByte(NetSwitch);
        writer.WriteByte(SubSwitch);
        writer.WriteInt16(Oem);
        writer.WriteByte(UbeaVersion);
        writer.WriteByte((byte)Status);
        writer.WriteInt16Reverse(EstaCode);
        writer.WriteString(ShortName, 18);
        writer.WriteString(LongName, 64);
        writer.WriteString(NodeReport, 64);
        writer.WriteInt16(PortCount);
        writer.WriteBytes(PortTypes);
        writer.WriteBytes(GoodInput);
        writer.WriteBytes(GoodOutputA);
        writer.WriteBytes(SwIn);
        writer.WriteBytes(SwOut);
        writer.WriteByte(AcnPriority);
        writer.WriteByte(SwMacro);
        writer.WriteByte(SwRemote);
        writer.WriteZeros(3);
        writer.WriteByte(Style);
        writer.WriteBytes(MacAddress);
        writer.WriteBytes(BindIpAddress);
        writer.WriteByte(BindIndex);
        writer.WriteByte(Status2);
        writer.WriteBytes(GoodOutputB);
        writer.WriteByte(Status3);
        writer.WriteBytes(respUID);
        writer.WriteZeros(15);
    }
}
