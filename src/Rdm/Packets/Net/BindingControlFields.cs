namespace Haukcode.ArtNet.Rdm.Packets.Net;

public class BindingControlFields
{
    public class Get : RdmRequestPacket
    {
        public Get()
            : base(RdmCommands.Get, RdmParameters.BindingControlFields)
        {
        }

        public UId Id { get; set; } = null!;

        protected override void ReadData(RdmBinaryReader data)
        {
            Id = data.ReadUId();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteUid(Id);
        }
    }

    public class GetReply : RdmResponsePacket
    {
        public GetReply()
            : base(RdmCommands.GetResponse, RdmParameters.BindingControlFields)
        {
        }

        public UId Id { get; set; } = null!;

        public ushort EndpointID { get; set; }

        public ushort ControlFields { get; set; }

        public UId BindingId { get; set; } = null!;

        protected override void ReadData(RdmBinaryReader data)
        {
            Id = data.ReadUId();
            EndpointID = data.ReadUInt16();
            ControlFields = data.ReadUInt16();
            BindingId = data.ReadUId();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteUid(Id);
            data.WriteUInt16(EndpointID);
            data.WriteUInt16(ControlFields);
            data.WriteUid(BindingId);
        }
    }
}
