namespace Haukcode.ArtNet.Packets;

/// <summary>Diagnostics priority codes (Art-Net 4 Table 5), used by ArtPoll and ArtDiagData.</summary>
public enum ArtDiagPriorities : byte
{
    Low = 0x10,
    Medium = 0x40,
    High = 0x80,
    Critical = 0xE0,

    /// <summary>Shown on a single line rather than in a list.</summary>
    Volatile = 0xF0
}

/// <summary>
/// Diagnostics text from a node or controller (Art-Net 4). Where it's sent is set by the
/// controller's ArtPoll (<see cref="ArtPollFlags.SendDiagnostics"/>, <see cref="ArtPollFlags.DiagnosticsUnicast"/>).
/// </summary>
public class ArtDiagDataPacket : ArtNetPacket
{
    /// <summary>Most text bytes, including the NUL terminator.</summary>
    public const int MaxDataLength = 512;

    public ArtDiagDataPacket()
        : base(ArtNetOpCodes.DiagData)
    {
    }

    public ArtDiagPriorities Priority { get; set; } = ArtDiagPriorities.Low;

    /// <summary>The logical DMX port the message is about; 0 for general messages.</summary>
    public byte LogicalPort { get; set; }

    public string Text { get; set; } = string.Empty;

    private byte[] EncodedText => ArtNetText.Encode(Text, MaxDataLength);

    // Filler1, DiagPriority, LogicalPort, Filler3, Length (2), Data
    protected override int DataLength => 6 + EncodedText.Length;

    internal static ArtDiagDataPacket Parse(BigEndianBinaryReader reader)
    {
        reader.SkipBytes(1);
        var target = new ArtDiagDataPacket
        {
            Priority = (ArtDiagPriorities)reader.ReadByte(),
            LogicalPort = reader.ReadByte()
        };
        reader.SkipBytes(1);

        int length = Math.Min(reader.ReadUInt16(), reader.BytesLeft);
        target.Text = ArtNetText.Decode(reader.ReadBytes(length));
        reader.SkipBytes(reader.BytesLeft);

        return target;
    }

    protected override void WriteData(ref SpanBinaryWriter writer)
    {
        var text = EncodedText;

        writer.WriteByte(0);
        writer.WriteByte((byte)Priority);
        writer.WriteByte(LogicalPort);
        writer.WriteByte(0);
        writer.WriteUInt16((ushort)text.Length);
        writer.WriteBytes(text);
    }
}
