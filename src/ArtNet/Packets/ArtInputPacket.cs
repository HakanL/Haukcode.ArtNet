namespace Haukcode.ArtNet.Packets;

public class ArtInputPacket : ArtNetPacket
{
    private byte[] inputs = { 0, 0, 0, 0 };
    public byte BindIndex { get; set; } = 0;

    public short NumPorts { get; set; } = 0;

    public byte[] Inputs
    {
        get => inputs;
        set
        {
            if (value.Length != 4)
                throw new ArgumentException("The input must be an array of 4 bytes.");

            this.inputs = value;
        }
    }

    protected override int DataLength => 8;

    public ArtInputPacket()
        : base(ArtNetOpCodes.Input)
    {
    }

    internal static ArtInputPacket Parse(BigEndianBinaryReader reader)
    {
        reader.SkipBytes(1);

        var target = new ArtInputPacket
        {
            BindIndex = reader.ReadByte(),
            NumPorts = reader.ReadInt16(),
            Inputs = reader.ReadBytes(4)
        };

        return target;
    }

    protected override void WriteData(BigEndianBinaryWriter writer)
    {
        writer.WriteByte((byte)0);
        writer.WriteByte(BindIndex);
        writer.WriteInt16(NumPorts);
        writer.WriteBytes(Inputs);
    }
}
