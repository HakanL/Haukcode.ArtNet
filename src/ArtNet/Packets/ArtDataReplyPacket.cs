namespace Haukcode.ArtNet.Packets;

/// <summary>
/// A node's answer to an <see cref="ArtDataRequestPacket"/> (Art-Net 4). Unicast only.
/// </summary>
public class ArtDataReplyPacket : ArtNetPacket
{
    public const int MaxPayloadLength = 512;

    public ArtDataReplyPacket()
        : base(ArtNetOpCodes.DataReply)
    {
    }

    public ushort EstaManufacturer { get; set; }

    public ushort Oem { get; set; }

    public ArtDataRequestCodes Request { get; set; }

    /// <summary>The reply data, up to 512 bytes. String data is NUL-terminated.</summary>
    public byte[] Payload { get; set; } = [];

    /// <summary>The payload as text, up to the first NUL.</summary>
    public string PayloadText
    {
        get => ArtNetText.Decode(Payload);
        set => Payload = ArtNetText.Encode(value, MaxPayloadLength);
    }

    // EstaMan (2), Oem (2), Request (2), PayLen (2), Payload
    protected override int DataLength => 8 + Payload.Length;

    internal static ArtDataReplyPacket Parse(BigEndianBinaryReader reader)
    {
        var target = new ArtDataReplyPacket
        {
            EstaManufacturer = reader.ReadUInt16(),
            Oem = reader.ReadUInt16(),
            Request = (ArtDataRequestCodes)reader.ReadUInt16()
        };

        int length = Math.Min(reader.ReadUInt16(), reader.BytesLeft);
        target.Payload = reader.ReadBytes(length);
        reader.SkipBytes(reader.BytesLeft);

        return target;
    }

    protected override void WriteData(ref SpanBinaryWriter writer)
    {
        if (Payload.Length > MaxPayloadLength)
            throw new InvalidOperationException($"ArtDataReply payload is at most {MaxPayloadLength} bytes.");

        writer.WriteUInt16(EstaManufacturer);
        writer.WriteUInt16(Oem);
        writer.WriteUInt16((ushort)Request);
        writer.WriteUInt16((ushort)Payload.Length);
        writer.WriteBytes(Payload);
    }
}
