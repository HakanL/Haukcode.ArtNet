namespace Haukcode.ArtNet.Packets;

public class ArtPollPacket : ArtNetPacket
{
    public ArtPollPacket()
        : base(ArtNetOpCodes.Poll)
    {
    }

    protected override int DataLength => 2;

    internal static ArtPollPacket Parse(BigEndianBinaryReader reader)
    {
        var target = new ArtPollPacket
        {
            TalkToMe = reader.ReadByte()
        };

        reader.SkipBytes(1);

        return target;
    }

    public byte TalkToMe { get; set; }

    protected override void WriteData(BigEndianBinaryWriter writer)
    {
        writer.WriteByte(TalkToMe);
        writer.WriteByte((byte)0);
    }
}
