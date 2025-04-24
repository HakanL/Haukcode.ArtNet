namespace Haukcode.ArtNet.Packets;

public class ArtAddressPacket : ArtNetPacket
{
    private byte[] swIn = { 0, 0, 0, 0 };
    private byte[] swOut = { 0, 0, 0, 0 };
    public byte NetSwitch { get; set; } = 0;
    public byte BindIndex { get; set; } = 0;
    public string ShortName { get; set; } = "";
    public string LongName { get; set; } = "";

    public byte[] SwIn
    {
        get => swIn;
        set
        {
            if (value.Length != 4)
                throw new ArgumentException("The SwIn must be an array of 4 bytes.");

            swIn = value;
        }
    }

    public byte[] SwOut
    {
        get => swOut;
        set
        {
            if (value.Length != 4)
                throw new ArgumentException("The SwOut must be an array of 4 bytes.");

            swOut = value;
        }
    }

    public byte SubSwitch { get; set; } = 0;
    public byte AcnPriority { get; set; } = 255;
    public ArtAddressCommand Command { get; set; } = ArtAddressCommand.AcNone;

    protected override int DataLength => 95;

    public ArtAddressPacket()
        : base(ArtNetOpCodes.Address)
    {
    }

    public ArtAddressPacket(ArtPollReplyPacket src)
        : base(ArtNetOpCodes.Address)
    {
        // Not all devices will be on code and set the address regardless of Bit 7. So better provide all the default values.
        BindIndex = src.BindIndex;
        SwIn = src.SwIn.Select(x => (byte)(x | 0x80)).ToArray();
        SwOut = src.SwOut.Select(x => (byte)(x | 0x80)).ToArray();
        SubSwitch = (byte)((src.SubSwitch & 0x000F) | 0x80);
        NetSwitch = (byte)(((src.SubSwitch & 0x7F00) >> 8) | 0x80);
    }

    internal static ArtAddressPacket Parse(BigEndianBinaryReader reader)
    {
        var target = new ArtAddressPacket
        {
            NetSwitch = reader.ReadByte(),
            BindIndex = reader.ReadByte(),
            ShortName = reader.ReadString(18),
            LongName = reader.ReadString(64),
            SwIn = reader.ReadBytes(4),
            SwOut = reader.ReadBytes(4),
            SubSwitch = reader.ReadByte(),
            AcnPriority = reader.ReadByte(),
            Command = (ArtAddressCommand)reader.ReadByte()
        };

        return target;
    }

    protected override void WriteData(BigEndianBinaryWriter writer)
    {
        writer.WriteByte(NetSwitch);
        writer.WriteByte(BindIndex);
        writer.WriteString(ShortName, 18);
        writer.WriteString(LongName, 64);
        writer.WriteBytes(SwIn);
        writer.WriteBytes(SwOut);
        writer.WriteByte(SubSwitch);
        writer.WriteByte(AcnPriority);
        writer.WriteByte((byte)Command);
    }
}