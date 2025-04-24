namespace Haukcode.ArtNet.Rdm.Packets.Net;

public class EndpointTimingDescription
{
    public class Get : RdmRequestPacket
    {
        public Get()
            : base(RdmCommands.Get, RdmParameters.EndpointTimingDescription)
        {
        }

        public byte SettingIndex { get; set; }

        protected override void ReadData(RdmBinaryReader data)
        {
            SettingIndex = data.ReadByte();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteByte(SettingIndex);
        }
    }

    public class GetReply : RdmResponsePacket
    {
        public GetReply()
            : base(RdmCommands.GetResponse, RdmParameters.EndpointTimingDescription)
        {
        }

        public byte SettingIndex { get; set; }

        public string Description { get; set; }

        protected override void ReadData(RdmBinaryReader data)
        {
            SettingIndex = data.ReadByte();
            Description = data.ReadString(Header.ParameterDataLength - 1);
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteByte(SettingIndex);
            data.WriteString(Description);
        }
    }
}
