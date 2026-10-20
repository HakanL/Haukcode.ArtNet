namespace Haukcode.ArtNet.Rdm.Packets.Net;

public class EndpointIdentify
{
    public class Get : RdmRequestPacket
    {
        public Get()
            : base(RdmCommands.Get, RdmParameters.EndpointIdentify)
        {
        }

        public ushort EndpointID { get; set; }

        protected override void ReadData(RdmBinaryReader data)
        {
            EndpointID = data.ReadUInt16();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteUInt16(EndpointID);
        }
    }

    public class GetReply : RdmResponsePacket
    {
        public GetReply()
            : base(RdmCommands.GetResponse, RdmParameters.EndpointIdentify)
        {
        }

        public ushort EndpointID { get; set; }

        public bool IdentifyOn { get; set; }

        protected override void ReadData(RdmBinaryReader data)
        {
            EndpointID = data.ReadUInt16();
            IdentifyOn = data.ReadBool();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteUInt16(EndpointID);
            data.WriteBool(IdentifyOn);
        }
    }

    public class Set : RdmRequestPacket
    {
        public Set()
            : base(RdmCommands.Set, RdmParameters.EndpointIdentify)
        {
        }

        public ushort EndpointID { get; set; }

        public bool IdentifyOn { get; set; }

        protected override void ReadData(RdmBinaryReader data)
        {
            EndpointID = data.ReadUInt16();
            IdentifyOn = data.ReadBool();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteUInt16(EndpointID);
            data.WriteBool(IdentifyOn);
        }
    }

    public class SetReply : RdmResponsePacket
    {
        public SetReply()
            : base(RdmCommands.SetResponse, RdmParameters.EndpointIdentify)
        {
        }

        public ushort EndpointID { get; set; }

        protected override void ReadData(RdmBinaryReader data)
        {
            EndpointID = data.ReadUInt16();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteUInt16(EndpointID);
        }
    }


}
