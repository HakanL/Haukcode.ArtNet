namespace Haukcode.ArtNet.Packets;

public class ArtTodDataPacket : ArtNetPacket
{
    public ArtTodDataPacket()
        : base(ArtNetOpCodes.TodData)
    {
        RdmVersion = 1;
        Devices = new List<UId>();
    }

    public byte RdmVersion { get; set; }

    public byte Port { get; set; }

    public byte BindIndex { get; set; }

    public byte Net { get; set; }

    public byte CommandResponse { get; set; } // 0x00 TodFull 0xFF TodNack

    public byte Universe { get; set; }

    public short UIdTotal { get; set; }

    public byte BlockCount { get; set; }

    public List<UId> Devices { get; set; }

    protected override int DataLength => 16 + Devices.Count * 6;

    internal static ArtTodDataPacket Parse(BigEndianBinaryReader reader)
    {
        var target = new ArtTodDataPacket
        {
            RdmVersion = reader.ReadByte(),
            Port = reader.ReadByte()
        };

        reader.SkipBytes(6);

        target.BindIndex = reader.ReadByte();
        target.Net = reader.ReadByte();
        target.CommandResponse = reader.ReadByte();
        target.Universe = reader.ReadByte();
        target.UIdTotal = reader.ReadInt16();
        target.BlockCount = reader.ReadByte();

        int count = reader.ReadByte();
        target.Devices = [];
        for (int n = 0; n < count; n++)
            target.Devices.Add(ReadUId(reader));

        return target;
    }

    protected override void WriteData(BigEndianBinaryWriter writer)
    {
        writer.WriteByte(RdmVersion);
        writer.WriteByte(Port);
        writer.WriteZeros(6);
        writer.WriteByte(BindIndex);
        writer.WriteByte(Net);
        writer.WriteByte(CommandResponse);
        writer.WriteByte(Universe);
        writer.WriteInt16(UIdTotal);
        writer.WriteByte(BlockCount);
        writer.WriteByte((byte)Devices.Count);

        foreach (UId id in Devices)
            WriteUid(writer, id);
    }
}
