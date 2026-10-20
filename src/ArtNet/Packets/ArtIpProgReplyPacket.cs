namespace Haukcode.ArtNet.Packets;

public class ArtIpProgReplyPacket : ArtNetPacket
{
    private byte[] defaultGateway = { 0, 0, 0, 0 };
    private byte[] netmask = { 255, 255, 255, 255 };
    private byte[] ipAddress = { 0, 0, 0, 0 };

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

    public bool DhcpEnabled { get; set; } = false;

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

    // Filler (4), IP (4), mask (4), port (2), status, spare, gateway (4), spare (2)
    protected override int DataLength => 22;

    public ArtIpProgReplyPacket()
        : base(ArtNetOpCodes.IpProgReply)
    {
    }

    internal static ArtIpProgReplyPacket Parse(BigEndianBinaryReader reader)
    {
        var target = new ArtIpProgReplyPacket();

        reader.SkipBytes(4);
        target.IpAddress = reader.ReadBytes(4);
        target.Netmask = reader.ReadBytes(4);
        reader.SkipBytes(2);
        target.DhcpEnabled = (reader.ReadByte() & (1 << 6)) != 0;
        reader.SkipBytes(1);
        target.DefaultGateway = reader.ReadBytes(4);
        reader.SkipBytes(Math.Min(2, reader.BytesLeft));

        return target;
    }

    protected override void WriteData(ref SpanBinaryWriter writer)
    {
        writer.WriteZeros(4);
        writer.WriteBytes(IpAddress);
        writer.WriteBytes(Netmask);
        writer.WriteUInt16((ushort)ArtNetClient.DefaultPort);
        writer.WriteByte((byte)(DhcpEnabled ? (1 << 6) : 0));
        writer.WriteZeros(1);
        writer.WriteBytes(DefaultGateway);
        writer.WriteZeros(2);
    }
}
