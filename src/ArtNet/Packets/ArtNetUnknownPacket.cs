namespace Haukcode.ArtNet.Packets;

public class ArtNetUnknownPacket : ArtNetPacket
{
    public ArtNetUnknownPacket(ArtNetOpCodes opCode)
        : base(opCode)
    {
    }

    protected override int DataLength => Data.Length;

    internal static ArtNetUnknownPacket Parse(ArtNetOpCodes opCode, BigEndianBinaryReader reader)
    {
        var target = new ArtNetUnknownPacket(opCode)
        {
            Data = reader.ReadBytes()
        };

        return target;
    }

    public byte[] Data { get; set; } = null!;

    protected override void WriteData(ref SpanBinaryWriter writer)
    {
        writer.WriteBytes(Data);
    }
}
