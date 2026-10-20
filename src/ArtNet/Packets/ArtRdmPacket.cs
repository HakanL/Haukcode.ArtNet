namespace Haukcode.ArtNet.Packets;

public class ArtRdmPacket : ArtNetPacket
{
    public ArtRdmPacket()
        : base(ArtNetOpCodes.Rdm)
    {
        RdmVersion = 1;
        SubStartCode = 1;
    }

    public byte RdmVersion { get; set; }

    /// <summary>
    /// How many more ArtRdm packets the node's queue can take (0 = not supported).
    /// </summary>
    public byte FifoAvail { get; set; }

    /// <summary>
    /// The node's queue size (0 = not supported).
    /// </summary>
    public byte FifoMax { get; set; }

    public byte Net { get; set; }

    public byte Command { get; set; }

    public byte Address { get; set; }

    public byte SubStartCode { get; set; }

    public byte[] RdmData { get; set; } = null!;

    protected override int DataLength => 13 + RdmData.Length;

    internal static ArtRdmPacket Parse(BigEndianBinaryReader reader)
    {
        var target = new ArtRdmPacket
        {
            RdmVersion = reader.ReadByte()
        };

        // Filler2, Spare1-5
        reader.SkipBytes(6);
        target.FifoAvail = reader.ReadByte();
        target.FifoMax = reader.ReadByte();
        target.Net = reader.ReadByte();
        target.Command = reader.ReadByte();
        target.Address = reader.ReadByte();
        target.SubStartCode = reader.ReadByte();
        target.RdmData = reader.ReadBytes();

        return target;
    }

    protected override void WriteData(ref SpanBinaryWriter writer)
    {
        writer.WriteByte(RdmVersion);
        writer.WriteZeros(6);
        writer.WriteByte(FifoAvail);
        writer.WriteByte(FifoMax);
        writer.WriteByte(Net);
        writer.WriteByte(Command);
        writer.WriteByte(Address);
        writer.WriteByte(SubStartCode);
        writer.WriteBytes(RdmData);
    }
}
