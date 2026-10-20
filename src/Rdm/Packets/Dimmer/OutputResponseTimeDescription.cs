namespace Haukcode.ArtNet.Rdm.Packets.Dimmer;

/// <summary>
/// This parameter is used to get a text description, up to 32 characters, of an output response
/// time setting (E1.37-1). GET only.
/// </summary>
public class OutputResponseTimeDescription
{
    public class Get : RdmRequestPacket
    {
        public Get()
            : base(RdmCommands.Get, RdmParameters.OutputResponseTimeDescription)
        {
        }

        public byte Setting { get; set; }

        protected override void ReadData(RdmBinaryReader data)
        {
            Setting = data.ReadByte();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteByte(Setting);
        }
    }

    public class GetReply : RdmResponsePacket
    {
        public GetReply()
            : base(RdmCommands.GetResponse, RdmParameters.OutputResponseTimeDescription)
        {
        }

        public byte Setting { get; set; }

        public string Description { get; set; } = string.Empty;

        protected override void ReadData(RdmBinaryReader data)
        {
            Setting = data.ReadByte();
            Description = data.ReadString(Header.ParameterDataLength - 1);
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteByte(Setting);
            data.WriteString(Description);
        }
    }
}
