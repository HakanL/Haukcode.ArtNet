namespace Haukcode.ArtNet.Rdm.Packets.Dimmer;

/// <summary>
/// This parameter is used to get a text description, up to 32 characters, of a dimmer curve
/// (E1.37-1). GET only.
/// </summary>
public class CurveDescription
{
    public class Get : RdmRequestPacket
    {
        public Get()
            : base(RdmCommands.Get, RdmParameters.CurveDescription)
        {
        }

        public byte CurveNumber { get; set; }

        protected override void ReadData(RdmBinaryReader data)
        {
            CurveNumber = data.ReadByte();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteByte(CurveNumber);
        }
    }

    public class GetReply : RdmResponsePacket
    {
        public GetReply()
            : base(RdmCommands.GetResponse, RdmParameters.CurveDescription)
        {
        }

        public byte CurveNumber { get; set; }

        public string Description { get; set; } = string.Empty;

        protected override void ReadData(RdmBinaryReader data)
        {
            CurveNumber = data.ReadByte();
            Description = data.ReadString(Header.ParameterDataLength - 1);
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteByte(CurveNumber);
            data.WriteString(Description);
        }
    }
}
