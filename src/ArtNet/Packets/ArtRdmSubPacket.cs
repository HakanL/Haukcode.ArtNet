namespace Haukcode.ArtNet.Packets;

public class ArtRdmSubPacket : ArtNetPacket
{
    public ArtRdmSubPacket()
        : base(ArtNetOpCodes.RdmSub)
    {
        RdmVersion = 1;
    }

    public byte RdmVersion { get; set; }

    public UId DeviceId { get; set; } = null!;

    public byte Command { get; set; }

    public ushort ParameterId { get; set; }

    public ushort SubDevice { get; set; }

    public ushort SubCount { get; set; }

    public byte[] RdmData { get; set; } = null!;

    protected override int DataLength => 20 + RdmData.Length;

    internal static ArtRdmSubPacket Parse(BigEndianBinaryReader reader)
    {
        var target = new ArtRdmSubPacket
        {
            RdmVersion = reader.ReadByte()
        };

        reader.SkipBytes(1);
        target.DeviceId = ReadUId(reader);
        reader.SkipBytes(1);
        target.Command = reader.ReadByte();
        target.ParameterId = reader.ReadUInt16();
        target.SubDevice = reader.ReadUInt16();
        target.SubCount = reader.ReadUInt16();
        reader.SkipBytes(4);

        target.RdmData = reader.ReadBytes();

        return target;
    }

    protected override void WriteData(ref SpanBinaryWriter writer)
    {
        writer.WriteByte(RdmVersion);
        writer.WriteByte(0x00);
        WriteUid(ref writer, DeviceId);
        writer.WriteByte(0x00);
        writer.WriteByte((byte)Command);
        writer.WriteUInt16(ParameterId);
        writer.WriteUInt16(SubDevice);
        // "Zero is illegal"
        if (SubCount == 0)
            throw new InvalidOperationException("ArtRdmSub SubCount must be at least 1.");

        writer.WriteUInt16(SubCount);
        writer.WriteZeros(4);
        writer.WriteBytes(RdmData);
    }
}
