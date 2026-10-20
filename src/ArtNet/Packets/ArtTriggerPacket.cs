namespace Haukcode.ArtNet.Packets;

public class ArtTriggerPacket : ArtNetPacket
{
    public ArtTriggerPacket()
        : base(ArtNetOpCodes.Trigger)
    {
    }

    public byte Filler1 { get; set; }

    public byte Filler2 { get; set; }

    public ushort OemCode { get; set; }

    public byte Key { get; set; }

    public byte SubKey { get; set; }

    /// <summary>Size of the fixed Data field.</summary>
    public const int DataSize = 512;

    /// <summary>
    /// Payload, up to 512 bytes; it's zero-padded to 512 on the wire.
    /// </summary>
    public byte[] Data { get; set; } = [];

    protected override int DataLength => 6 + DataSize;

    internal static ArtTriggerPacket Parse(BigEndianBinaryReader reader)
    {
        var target = new ArtTriggerPacket
        {
            Filler1 = reader.ReadByte(),
            Filler2 = reader.ReadByte(),
            OemCode = reader.ReadUInt16(),
            Key = reader.ReadByte(),
            SubKey = reader.ReadByte(),
            Data = reader.ReadBytes(Math.Min(DataSize, reader.BytesLeft))
        };

        return target;
    }

    protected override void WriteData(ref SpanBinaryWriter writer)
    {
        writer.WriteByte(Filler1);
        writer.WriteByte(Filler2);
        writer.WriteUInt16(OemCode);
        writer.WriteByte(Key);
        writer.WriteByte(SubKey);

        if (Data.Length > DataSize)
            throw new InvalidOperationException($"ArtTrigger data is at most {DataSize} bytes.");

        writer.WriteBytes(Data);
        writer.WriteZeros(DataSize - Data.Length);
    }
}
