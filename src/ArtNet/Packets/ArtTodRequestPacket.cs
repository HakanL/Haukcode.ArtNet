namespace Haukcode.ArtNet.Packets;

public class ArtTodRequestPacket : ArtNetPacket
{
    public ArtTodRequestPacket()
        : base(ArtNetOpCodes.TodRequest)
    {
        RequestedUniverses = new List<byte>();
    }

    public byte Net { get; set; }

    public byte Command { get; set; }

    public List<byte> RequestedUniverses { get; set; }

    protected override int DataLength => 12 + RequestedUniverses.Count;

    internal static ArtTodRequestPacket Parse(BigEndianBinaryReader reader)
    {
        reader.SkipBytes(9);

        var target = new ArtTodRequestPacket
        {
            Net = reader.ReadByte(),
            Command = reader.ReadByte()
        };

        int count = reader.ReadByte();

        target.RequestedUniverses = new List<byte>(reader.ReadBytes(count));

        return target;
    }

    protected override void WriteData(BigEndianBinaryWriter writer)
    {
        writer.WriteZeros(9);
        writer.WriteByte(Net);
        writer.WriteByte(Command);
        writer.WriteByte((byte)RequestedUniverses.Count);
        writer.WriteBytes(RequestedUniverses.ToArray());
    }
}
