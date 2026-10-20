namespace Haukcode.ArtNet.Packets;

/// <summary>ArtFirmwareReply result codes.</summary>
public enum ArtFirmwareReplyTypes : byte
{
    BlockGood = 0x00,
    AllGood = 0x01,
    Fail = 0xFF
}

/// <summary>
/// A node's acknowledgement of each <see cref="ArtFirmwareMasterPacket"/> block (Art-Net 4).
/// Unicast only.
/// </summary>
public class ArtFirmwareReplyPacket : ArtNetPacket
{
    public ArtFirmwareReplyPacket()
        : base(ArtNetOpCodes.FirmwareReply)
    {
    }

    public ArtFirmwareReplyTypes Type { get; set; }

    // Filler (2), Type, Spare (21)
    protected override int DataLength => 24;

    internal static ArtFirmwareReplyPacket Parse(BigEndianBinaryReader reader)
    {
        reader.SkipBytes(2);
        var target = new ArtFirmwareReplyPacket
        {
            Type = (ArtFirmwareReplyTypes)reader.ReadByte()
        };
        reader.SkipBytes(reader.BytesLeft);

        return target;
    }

    protected override void WriteData(ref SpanBinaryWriter writer)
    {
        writer.WriteZeros(2);
        writer.WriteByte((byte)Type);
        writer.WriteZeros(21);
    }
}
