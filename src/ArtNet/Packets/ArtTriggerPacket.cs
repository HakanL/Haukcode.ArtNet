namespace Haukcode.ArtNet.Packets;

public class ArtTriggerPacket : ArtNetPacket
{
    public ArtTriggerPacket()
        : base(ArtNetOpCodes.Trigger)
    {
    }

    public byte Filler1 { get; set; }

    public byte Filler2 { get; set; }

    public Int16 OemCode { get; set; }

    public byte Key { get; set; }

    public byte SubKey { get; set; }

    public byte[] Data { get; set; } = null!;

    protected override int DataLength => 6 + Data.Length;

    internal static ArtTriggerPacket Parse(BigEndianBinaryReader reader)
    {
        var target = new ArtTriggerPacket
        {
            Filler1 = reader.ReadByte(),
            Filler2 = reader.ReadByte(),
            OemCode = reader.ReadInt16(),
            Key = reader.ReadByte(),
            SubKey = reader.ReadByte(),
            Data = reader.ReadBytes(512)
        };

        return target;
    }

    protected override void WriteData(BigEndianBinaryWriter writer)
    {
        writer.WriteByte(Filler1);
        writer.WriteByte(Filler2);
        writer.WriteInt16(OemCode);
        writer.WriteByte(Key);
        writer.WriteByte(SubKey);
        writer.WriteBytes(Data);
    }
}
