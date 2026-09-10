namespace Haukcode.ArtNet.Packets;

/// <summary>
/// ArtTimeCode (OpCode 0x9700). Transports SMPTE time over Art-Net UDP.
/// Layout after the 12-byte header: Filler1, StreamId, Frames, Seconds,
/// Minutes, Hours, Type. Total packet is 19 bytes.
/// </summary>
public class ArtTimeCodePacket : ArtNetPacket
{
    public ArtTimeCodePacket()
        : base(ArtNetOpCodes.TimeCode)
    {
    }

    /// <summary>Ignored by receiver; sender sets to zero.</summary>
    public byte Filler1 { get; set; }

    /// <summary>0 is the master stream; other values identify extra streams.</summary>
    public byte StreamId { get; set; }

    /// <summary>0–29 depending on <see cref="Type"/>.</summary>
    public byte Frames { get; set; }

    /// <summary>0–59.</summary>
    public byte Seconds { get; set; }

    /// <summary>0–59.</summary>
    public byte Minutes { get; set; }

    /// <summary>0–23.</summary>
    public byte Hours { get; set; }

    public ArtTimeCodeTypes Type { get; set; }

    protected override int DataLength => 7;

    internal static ArtTimeCodePacket Parse(BigEndianBinaryReader reader)
    {
        var target = new ArtTimeCodePacket
        {
            Filler1 = reader.ReadByte(),
            StreamId = reader.ReadByte(),
            Frames = reader.ReadByte(),
            Seconds = reader.ReadByte(),
            Minutes = reader.ReadByte(),
            Hours = reader.ReadByte(),
            Type = (ArtTimeCodeTypes)reader.ReadByte()
        };

        // Some senders pad the datagram; consume leftovers so Parse does not
        // Debug.Assert on PacketLength vs BytesRead.
        if (reader.BytesLeft > 0)
            reader.SkipBytes(reader.BytesLeft);

        return target;
    }

    protected override void WriteData(ref SpanBinaryWriter writer)
    {
        writer.WriteByte(Filler1);
        writer.WriteByte(StreamId);
        writer.WriteByte(Frames);
        writer.WriteByte(Seconds);
        writer.WriteByte(Minutes);
        writer.WriteByte(Hours);
        writer.WriteByte((byte)Type);
    }
}
