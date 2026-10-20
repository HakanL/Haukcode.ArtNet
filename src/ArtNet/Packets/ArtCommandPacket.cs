namespace Haukcode.ArtNet.Packets;

/// <summary>
/// Text property commands, "Command=Data&amp;" pairs, case-insensitive (Art-Net 4). Unicast or
/// broadcast, as the application decides.
/// </summary>
public class ArtCommandPacket : ArtNetPacket
{
    /// <summary>Most text bytes, including the NUL terminator.</summary>
    public const int MaxDataLength = 512;

    public ArtCommandPacket()
        : base(ArtNetOpCodes.Command)
    {
    }

    public ushort EstaManufacturer { get; set; }

    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// The commands in <see cref="Text"/> as name/value pairs.
    /// </summary>
    public IEnumerable<KeyValuePair<string, string>> Commands =>
        Text.Split('&', StringSplitOptions.RemoveEmptyEntries)
            .Select(c => c.Split('=', 2))
            .Select(p => new KeyValuePair<string, string>(p[0], p.Length > 1 ? p[1] : string.Empty));

    private byte[] EncodedText => ArtNetText.Encode(Text, MaxDataLength);

    // EstaMan (2), Length (2), Data
    protected override int DataLength => 4 + EncodedText.Length;

    internal static ArtCommandPacket Parse(BigEndianBinaryReader reader)
    {
        var target = new ArtCommandPacket
        {
            EstaManufacturer = reader.ReadUInt16()
        };

        int length = Math.Min(reader.ReadUInt16(), reader.BytesLeft);
        target.Text = ArtNetText.Decode(reader.ReadBytes(length));
        reader.SkipBytes(reader.BytesLeft);

        return target;
    }

    protected override void WriteData(ref SpanBinaryWriter writer)
    {
        var text = EncodedText;

        writer.WriteUInt16(EstaManufacturer);
        writer.WriteUInt16((ushort)text.Length);
        writer.WriteBytes(text);
    }
}
