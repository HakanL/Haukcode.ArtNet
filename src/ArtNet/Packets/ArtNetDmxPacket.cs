namespace Haukcode.ArtNet.Packets;

public class ArtNetDmxPacket : ArtNetPacket
{
    public ArtNetDmxPacket()
        : base(ArtNetOpCodes.Output)
    {
    }

    public byte Sequence { get; set; }

    public byte Physical { get; set; }

    public ushort Universe { get; set; }

    public ReadOnlyMemory<byte> DmxData { get; set; }

    /// <summary>Maximum number of DMX slots in one packet.</summary>
    public const int MaxDmxLength = 512;

    // The spec requires an even length of 2 to 512; shorter or odd data is zero-padded
    private int PaddedDmxLength => Math.Max(2, DmxData.Length + (DmxData.Length & 1));

    protected override int DataLength => 6 + PaddedDmxLength;

    internal static ArtNetDmxPacket Parse(BigEndianBinaryReader reader)
    {
        return Parse(reader, new ArtNetDmxPacket());
    }

    /// <summary>
    /// Parse into <paramref name="target"/>; every field is rewritten, so a reused instance
    /// carries nothing over from the previous packet.
    /// </summary>
    internal static ArtNetDmxPacket Parse(BigEndianBinaryReader reader, ArtNetDmxPacket target)
    {
        target.Sequence = reader.ReadByte();
        target.Physical = reader.ReadByte();
        target.Universe = reader.ReadUInt16Reverse();

        int length = reader.ReadUInt16();
        if (length > reader.BytesLeft)
            throw new InvalidDataException($"ArtDmx length {length} is longer than the {reader.BytesLeft} bytes received.");

        // Zero-copy slice over the receive buffer instead of ReadBytes().ToArray(); the
        // consumer copies it into a pooled buffer synchronously before the buffer is reused.
        target.DmxData = reader.ReadSlice(length);

        return target;
    }

    protected override void WriteData(ref SpanBinaryWriter writer)
    {
        writer.WriteByte(Sequence);
        writer.WriteByte(Physical);
        writer.WriteUInt16Reverse(Universe);
        if (DmxData.Length > MaxDmxLength)
            throw new InvalidOperationException($"ArtDmx data is at most {MaxDmxLength} bytes.");

        int length = PaddedDmxLength;
        writer.WriteUInt16((ushort)length);
        writer.WriteBytes(DmxData);
        writer.WriteZeros(length - DmxData.Length);
    }
}
