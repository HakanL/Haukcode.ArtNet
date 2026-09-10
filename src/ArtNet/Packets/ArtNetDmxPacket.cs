namespace Haukcode.ArtNet.Packets;

public class ArtNetDmxPacket : ArtNetPacket
{
    public ArtNetDmxPacket()
        : base(ArtNetOpCodes.Output)
    {
    }

    public byte Sequence { get; set; }

    public byte Physical { get; set; }

    public short Universe { get; set; }

    public ReadOnlyMemory<byte> DmxData { get; set; }

    protected override int DataLength => 6 + DmxData.Length;

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
        target.Universe = reader.ReadInt16Reverse();

        int length = reader.ReadInt16();
        // Zero-copy slice over the receive buffer instead of ReadBytes().ToArray(); the
        // consumer copies it into a pooled buffer synchronously before the buffer is reused.
        target.DmxData = reader.ReadSlice(length);

        return target;
    }

    protected override void WriteData(BigEndianBinaryWriter writer)
    {
        writer.WriteByte(Sequence);
        writer.WriteByte(Physical);
        writer.WriteInt16Reverse(Universe);
        writer.WriteInt16((short)DmxData.Length);
        writer.WriteBytes(DmxData);
    }
}
