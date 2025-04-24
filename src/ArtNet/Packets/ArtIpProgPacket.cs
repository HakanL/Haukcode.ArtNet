namespace Haukcode.ArtNet.Packets;

public class ArtIpProgPacket : ArtNetPacket
{
    private byte[] defaultGateway = { 0, 0, 0, 0 };
    private byte[] netmask = { 255, 255, 255, 255 };
    private byte[] ipAddress = { 0, 0, 0, 0 };

    public ArtIpProgCommand Command { get; set; } = ArtIpProgCommand.None;

    public byte[] IpAddress
    {
        get => ipAddress;
        set
        {
            if (value.Length != 4)
                throw new ArgumentException("The IpAddress must be an array of 4 bytes.");

            ipAddress = value;
        }
    }

    public byte[] Netmask
    {
        get => netmask;
        set
        {
            if (value.Length != 4)
                throw new ArgumentException("The netmask must be an array of 4 bytes.");

            netmask = value;
        }
    }

    public byte[] DefaultGateway
    {
        get => defaultGateway;
        set
        {
            if (value.Length != 4)
                throw new ArgumentException("The default gateway must be an array of 4 bytes.");

            defaultGateway = value;
        }
    }

    protected override int DataLength => 18;

    public ArtIpProgPacket()
        : base(ArtNetOpCodes.IpProg)
    {
    }

    internal static ArtIpProgPacket Parse(BigEndianBinaryReader reader)
    {
        var target = new ArtIpProgPacket();

        reader.SkipBytes(2);
        target.Command = (ArtIpProgCommand)reader.ReadByte();
        reader.SkipBytes(1);
        target.IpAddress = reader.ReadBytes(4);
        target.Netmask = reader.ReadBytes(4);
        reader.SkipBytes(2);
        target.DefaultGateway = reader.ReadBytes(4);

        return target;
    }

    protected override void WriteData(BigEndianBinaryWriter writer)
    {
        writer.WriteZeros(2);
        writer.WriteByte((byte)Command);
        writer.WriteByte(0x00);
        writer.WriteBytes(IpAddress);
        writer.WriteBytes(Netmask);
        writer.WriteInt16((short)ArtNetClient.DefaultPort);
        writer.WriteBytes(DefaultGateway);
    }
}
