namespace Haukcode.ArtNet.Rdm.Packets.Net;

public class EndpointLabel
{
    public class Get : RdmRequestPacket
    {
        public Get()
            : base(RdmCommands.Get, RdmParameters.EndpointLabel)
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
            : base(RdmCommands.GetResponse, RdmParameters.EndpointLabel)
        {
        }

        public ushort EndpointID { get; set; }

        public string Label { get; set; } = null!;

        protected override void ReadData(RdmBinaryReader data)
        {
            EndpointID = data.ReadUInt16();
            Label = Encoding.ASCII.GetString(data.ReadBytes(Header.ParameterDataLength-2));
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteUInt16(EndpointID);
            data.WriteString(Label);
        }
    }

    public class Set : RdmRequestPacket
    {
        public Set()
            : base(RdmCommands.Set, RdmParameters.EndpointLabel)
        {
        }

        public ushort EndpointID { get; set; }

        public string Label { get; set; } = null!;

        protected override void ReadData(RdmBinaryReader data)
        {
            EndpointID = data.ReadUInt16();
            Label = Encoding.ASCII.GetString(data.ReadBytes(Header.ParameterDataLength - 2));
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteUInt16(EndpointID);
            data.WriteString(Label);
        }
    }

    public class SetReply : RdmResponsePacket
    {
        public SetReply()
            : base(RdmCommands.SetResponse, RdmParameters.EndpointLabel)
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
