namespace Haukcode.ArtNet.Packets;

/// <summary>ArtFirmwareMaster block types.</summary>
public enum ArtFirmwareTypes : byte
{
    FirmFirst = 0x00,
    FirmCont = 0x01,
    FirmLast = 0x02,
    UbeaFirst = 0x03,
    UbeaCont = 0x04,
    UbeaLast = 0x05
}

/// <summary>
/// One block of a firmware or UBEA upload to a node (Art-Net 4). Unicast only; the node
/// acknowledges each block with an <see cref="ArtFirmwareReplyPacket"/>.
/// </summary>
public class ArtFirmwareMasterPacket : ArtNetPacket
{
    /// <summary>Bytes per block: 512 16-bit words.</summary>
    public const int BlockSize = 1024;

    public ArtFirmwareMasterPacket()
        : base(ArtNetOpCodes.FirmwareMaster)
    {
    }

    public ArtFirmwareTypes Type { get; set; }

    /// <summary>Counts the blocks, starting at 0 for the first one.</summary>
    public byte BlockId { get; set; }

    /// <summary>Total upload size in 16-bit words, including the firmware header (the .alf/.alu file size in words).</summary>
    public uint FirmwareLength { get; set; }

    /// <summary>Up to 1024 bytes of firmware data, high byte of each word first; zero-padded to a full block.</summary>
    public byte[] Data { get; set; } = [];

    // Filler (2), Type, BlockId, FirmwareLength (4), Spare (20), Data (1024)
    protected override int DataLength => 28 + BlockSize;

    internal static ArtFirmwareMasterPacket Parse(BigEndianBinaryReader reader)
    {
        reader.SkipBytes(2);
        var target = new ArtFirmwareMasterPacket
        {
            Type = (ArtFirmwareTypes)reader.ReadByte(),
            BlockId = reader.ReadByte(),
            FirmwareLength = reader.ReadUInt32()
        };
        reader.SkipBytes(20);
        target.Data = reader.ReadBytes(Math.Min(BlockSize, reader.BytesLeft));
        reader.SkipBytes(reader.BytesLeft);

        return target;
    }

    protected override void WriteData(ref SpanBinaryWriter writer)
    {
        if (Data.Length > BlockSize)
            throw new InvalidOperationException($"An ArtFirmwareMaster block is at most {BlockSize} bytes.");

        writer.WriteZeros(2);
        writer.WriteByte((byte)Type);
        writer.WriteByte(BlockId);
        writer.WriteUInt32(FirmwareLength);
        writer.WriteZeros(20);
        writer.WriteBytes(Data);
        writer.WriteZeros(BlockSize - Data.Length);
    }
}
