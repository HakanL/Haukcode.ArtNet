namespace Haukcode.ArtNet.Packets;

/// <summary>
/// DMX512 data with a non-zero start code, other than RDM (Art-Net 4). Unicast only. A packet
/// with the VLC start code and magic numbers is parsed as <see cref="ArtVlcPacket"/>.
/// </summary>
public class ArtNzsPacket : ArtNetPacket
{
    public const int MaxDataLength = 512;

    public ArtNzsPacket()
        : base(ArtNetOpCodes.Nzs)
    {
    }

    /// <summary>1-255 to let the receiver resequence packets; 0 disables sequencing.</summary>
    public byte Sequence { get; set; }

    /// <summary>The DMX512 start code; must not be 0 or RDM (0xCC).</summary>
    public byte StartCode { get; set; }

    /// <summary>15-bit Port-Address (SubUni in the low byte, Net in the high byte).</summary>
    public ushort Universe { get; set; }

    /// <summary>1 to 512 slots.</summary>
    public virtual ReadOnlyMemory<byte> Data { get; set; }

    // Sequence, StartCode, SubUni, Net, Length (2), Data
    protected override int DataLength => 6 + Data.Length;

    internal static ArtNzsPacket Parse(BigEndianBinaryReader reader)
    {
        byte sequence = reader.ReadByte();
        byte startCode = reader.ReadByte();
        ushort universe = reader.ReadUInt16Reverse();

        int length = reader.ReadUInt16();
        if (length > reader.BytesLeft)
            throw new InvalidDataException($"ArtNzs length {length} is longer than the {reader.BytesLeft} bytes received.");

        var data = reader.ReadBytes(length);

        if (ArtVlcPacket.IsVlc(startCode, data))
            return ArtVlcPacket.FromNzs(sequence, universe, data);

        return new ArtNzsPacket { Sequence = sequence, StartCode = startCode, Universe = universe, Data = data };
    }

    protected override void WriteData(ref SpanBinaryWriter writer)
    {
        if (StartCode == 0 || StartCode == (byte)DmxStartCodes.RDM)
            throw new InvalidOperationException("ArtNzs is for non-zero start codes other than RDM; use ArtDmx or ArtRdm.");

        var data = Data;
        if (data.Length < 1 || data.Length > MaxDataLength)
            throw new InvalidOperationException($"ArtNzs carries 1 to {MaxDataLength} slots.");

        writer.WriteByte(Sequence);
        writer.WriteByte(StartCode);
        writer.WriteUInt16Reverse(Universe);
        writer.WriteUInt16((ushort)data.Length);
        writer.WriteBytes(data);
    }
}
