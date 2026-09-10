namespace Haukcode.ArtNet.Packets;

public class ArtSyncPacket : ArtNetPacket
{
    public ArtSyncPacket()
        : base(ArtNetOpCodes.Sync)
    {
    }

    public short Aux { get; set; }

    protected override int DataLength => 2;

    internal static ArtSyncPacket Parse(BigEndianBinaryReader reader)
    {
        var target = new ArtSyncPacket();

        // The spec defines Aux1/Aux2 as "transmitted as zero" spare bytes, and some
        // controllers omit them entirely (12-byte ArtSync). Treat them as optional
        // instead of throwing on every sync frame from such a sender.
        if (reader.BytesLeft >= 2)
            target.Aux = reader.ReadInt16Reverse();
        else
            reader.SkipBytes(reader.BytesLeft);

        return target;
    }

    protected override void WriteData(ref SpanBinaryWriter writer)
    {
        writer.WriteInt16(Aux);
    }
}
