namespace Haukcode.ArtNet.Packets;

/// <summary>DataRequest codes (Art-Net 4 Table 4a), used by ArtDataRequest and ArtDataReply.</summary>
public enum ArtDataRequestCodes : ushort
{
    /// <summary>Polls whether ArtDataRequest is supported.</summary>
    Poll = 0x0000,
    UrlProduct = 0x0001,
    UrlUserGuide = 0x0002,
    UrlSupport = 0x0003,
    UrlPersonalityUdr = 0x0004,
    UrlPersonalityGdtf = 0x0005,

    /// <summary>Start of the manufacturer-specific range (0x8000-0xFFFF).</summary>
    ManufacturerSpecific = 0x8000
}

/// <summary>
/// Requests data such as a product URL from a node (Art-Net 4). Unicast only; the node answers
/// with an <see cref="ArtDataReplyPacket"/>.
/// </summary>
public class ArtDataRequestPacket : ArtNetPacket
{
    public ArtDataRequestPacket()
        : base(ArtNetOpCodes.DataRequest)
    {
    }

    public ushort EstaManufacturer { get; set; }

    public ushort Oem { get; set; }

    public ArtDataRequestCodes Request { get; set; }

    // EstaMan (2), Oem (2), Request (2), Spare (22)
    protected override int DataLength => 28;

    internal static ArtDataRequestPacket Parse(BigEndianBinaryReader reader)
    {
        var target = new ArtDataRequestPacket
        {
            EstaManufacturer = reader.ReadUInt16(),
            Oem = reader.ReadUInt16(),
            Request = (ArtDataRequestCodes)reader.ReadUInt16()
        };

        reader.SkipBytes(reader.BytesLeft);

        return target;
    }

    protected override void WriteData(ref SpanBinaryWriter writer)
    {
        writer.WriteUInt16(EstaManufacturer);
        writer.WriteUInt16(Oem);
        writer.WriteUInt16((ushort)Request);
        writer.WriteZeros(22);
    }
}
