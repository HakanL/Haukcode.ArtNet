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

    public ushort Port { get; set; } = ArtNetClient.DefaultPort;

    public ushort FirmwareVersion { get; set; }

    public byte NetSwitch { get; set; }
    
    public byte SubSwitch { get; set; }
    
    public ushort Oem { get; set; } = 0xff;

    public byte UbeaVersion { get; set; }

    public byte Status { get; set; }

    public ushort EstaCode { get; set; }

    public string ShortName { get; set; } = string.Empty;

    public string LongName { get; set; } = string.Empty;

    public string NodeReport { get; set; } = string.Empty;

    public ushort PortCount { get; set; }

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

    private byte[] swIn = new byte[4];

    public byte[] SwIn
    {
        get => swIn;
        set
        {
            if (value.Length != 4)
                throw new ArgumentException("SwIn must be an array of 4 bytes.");

            swIn = value;
        }
    }

    private byte[] swOut = new byte[4];

    public byte[] SwOut
    {
        get => swOut;
        set
        {
            if (value.Length != 4)
                throw new ArgumentException("SwOut must be an array of 4 bytes.");

            swOut = value;
        }
    }

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

    /// <summary>Data available for user-specific purposes.</summary>
    public ushort User { get; set; }

    /// <summary>
    /// Highest ArtDmx refresh rate the node can process, in Hz; 0 to 44 means the DMX512 maximum
    /// of 44 Hz.
    /// </summary>
    public ushort RefreshRate { get; set; }

    /// <summary>
    /// How the node collects status and queued messages from its RDM devices (values 5-250 are
    /// manufacturer-defined). Only used when <see cref="Status3Flags"/> has
    /// <see cref="PollReplyStatus3.BackgroundQueue"/>.
    /// </summary>
    public BackgroundQueuePolicies BackgroundQueuePolicy { get; set; }

    public IndicatorStates IndicatorState => (IndicatorStates)(Status >> 6);

    public PortAddressAuthorities PortAddressAuthority => (PortAddressAuthorities)((Status >> 4) & 0x03);

    public PollReplyStatus1 Status1Flags => (PollReplyStatus1)(Status & 0x07);

    public PollReplyStatus2 Status2Flags => (PollReplyStatus2)Status2;

    public FailsafeStates FailsafeState => (FailsafeStates)(Status3 >> 6);

    public PollReplyStatus3 Status3Flags => (PollReplyStatus3)(Status3 & 0x3F);

    public GoodInputFlags GetGoodInput(int port) => (GoodInputFlags)GoodInput[port];

    public GoodOutputAFlags GetGoodOutputA(int port) => (GoodOutputAFlags)GoodOutputA[port];

    public GoodOutputBFlags GetGoodOutputB(int port) => (GoodOutputBFlags)GoodOutputB[port];

    protected override int DataLength => 229;

    /// <summary>
    /// Interprets the universe address to ensure compatibility with ArtNet I, II and III devices.
    /// </summary>
    /// <param name="outPorts">Whether to get the address for in or out ports.</param>
    /// <param name="portIndex">The port index to obtain the universe for.</param>
    /// <returns>The 15 Bit universe address</returns>
    public int UniverseAddress(bool outPorts, int portIndex)
    {
        // 15-bit Port-Address: Net (7 bits), Sub-Net (4 bits), Universe (4 bits). Only the low
        // nibble of SwIn/SwOut is the universe, whatever the Net and Sub-Net are.
        int universe = (NetSwitch & 0x7F) << 8;
        universe += (SubSwitch & 0x0F) << 4;
        universe += (outPorts ? SwOut[portIndex] : SwIn[portIndex]) & 0x0F;

        return universe;
    }

    internal static ArtPollReplyPacket Parse(BigEndianBinaryReader reader)
    {
        var target = new ArtPollReplyPacket();

        target.IpAddress = reader.ReadBytes(4);
        target.Port = reader.ReadUInt16Reverse();
        target.FirmwareVersion = reader.ReadUInt16();
        target.NetSwitch = reader.ReadByte();
        target.SubSwitch = reader.ReadByte();
        target.Oem = reader.ReadUInt16();
        target.UbeaVersion = reader.ReadByte();
        target.Status = reader.ReadByte();
        target.EstaCode = reader.ReadUInt16Reverse();
        target.ShortName = reader.ReadString(18);
        target.LongName = reader.ReadString(64);
        target.NodeReport = reader.ReadString(64);
        target.PortCount = reader.ReadUInt16();
        target.PortTypes = reader.ReadBytes(4);
        target.GoodInput = reader.ReadBytes(4);
        target.GoodOutputA = reader.ReadBytes(4);
        target.SwIn = reader.ReadBytes(4);
        target.SwOut = reader.ReadBytes(4);
        target.AcnPriority = reader.ReadByte();
        target.SwMacro = reader.ReadByte();
        target.SwRemote = reader.ReadByte();

        reader.SkipBytes(3);

        // The fields below were added over successive revisions of the Art-Net spec
        // (BindIp/Status2 in Art-Net 3, GoodOutputB/Status3/RespUID in Art-Net 4).
        // Legacy nodes send shorter replies, so stop cleanly when the packet ends.
        if (reader.BytesLeft >= 1)
            target.Style = reader.ReadByte();
        if (reader.BytesLeft >= 6)
            target.MacAddress = reader.ReadBytes(6);
        if (reader.BytesLeft >= 4)
            target.BindIpAddress = reader.ReadBytes(4);
        if (reader.BytesLeft >= 1)
            target.BindIndex = reader.ReadByte();
        if (reader.BytesLeft >= 1)
            target.Status2 = reader.ReadByte();
        if (reader.BytesLeft >= 4)
            target.GoodOutputB = reader.ReadBytes(4);
        if (reader.BytesLeft >= 1)
            target.Status3 = reader.ReadByte();
        if (reader.BytesLeft >= 6)
            target.RespUID = reader.ReadBytes(6);
        if (reader.BytesLeft >= 2)
            target.User = reader.ReadUInt16();
        if (reader.BytesLeft >= 2)
            target.RefreshRate = reader.ReadUInt16();
        if (reader.BytesLeft >= 1)
            target.BackgroundQueuePolicy = (BackgroundQueuePolicies)reader.ReadByte();

        // Consume the trailing filler (10 spare bytes in a full-length packet, or
        // whatever a nonstandard node appended) so the datagram is fully read.
        reader.SkipBytes(reader.BytesLeft);

        return target;
    }

    protected override void WriteData(ref SpanBinaryWriter writer)
    {
        writer.WriteBytes(IpAddress);
        writer.WriteUInt16Reverse(Port);
        writer.WriteUInt16(FirmwareVersion);
        writer.WriteByte(NetSwitch);
        writer.WriteByte(SubSwitch);
        writer.WriteUInt16(Oem);
        writer.WriteByte(UbeaVersion);
        writer.WriteByte((byte)Status);
        writer.WriteUInt16Reverse(EstaCode);
        writer.WriteString(ShortName, 18);
        writer.WriteString(LongName, 64);
        writer.WriteString(NodeReport, 64);
        writer.WriteUInt16(PortCount);
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
        writer.WriteUInt16(User);
        writer.WriteUInt16(RefreshRate);
        writer.WriteByte((byte)BackgroundQueuePolicy);
        writer.WriteZeros(10);
    }
}
