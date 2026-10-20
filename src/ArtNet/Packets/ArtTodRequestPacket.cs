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

    /// <summary>Size of the fixed Address array.</summary>
    public const int MaxRequestedUniverses = 32;

    protected override int DataLength => 12 + MaxRequestedUniverses;

    internal static ArtTodRequestPacket Parse(BigEndianBinaryReader reader)
    {
        reader.SkipBytes(9);

        var target = new ArtTodRequestPacket
        {
            Net = reader.ReadByte(),
            Command = reader.ReadByte()
        };

        int count = Math.Min((int)reader.ReadByte(), MaxRequestedUniverses);
        var addresses = reader.ReadBytes(Math.Min(MaxRequestedUniverses, reader.BytesLeft));

        target.RequestedUniverses = new List<byte>(addresses.Take(count));

        return target;
    }

    protected override void WriteData(ref SpanBinaryWriter writer)
    {
        writer.WriteZeros(9);
        writer.WriteByte(Net);
        writer.WriteByte(Command);
        if (RequestedUniverses.Count > MaxRequestedUniverses)
            throw new InvalidOperationException($"An ArtTodRequest can request at most {MaxRequestedUniverses} universes.");

        writer.WriteByte((byte)RequestedUniverses.Count);
        writer.WriteBytes(RequestedUniverses.ToArray());
        writer.WriteZeros(MaxRequestedUniverses - RequestedUniverses.Count);
    }
}
