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
            TalkToMe = reader.ReadByte(),
            DiagPriority = reader.BytesLeft > 0 ? reader.ReadByte() : (byte)0
        };

        // Art-Net 4 adds optional targeted-mode, EstaMan and Oem fields; they aren't modelled
        reader.SkipBytes(reader.BytesLeft);

        return target;
    }

    public byte TalkToMe { get; set; }

    /// <summary>
    /// The lowest priority of diagnostics messages that should be sent.
    /// </summary>
    public byte DiagPriority { get; set; }

    protected override void WriteData(ref SpanBinaryWriter writer)
    {
        writer.WriteByte(TalkToMe);
        writer.WriteByte(DiagPriority);
    }
}
