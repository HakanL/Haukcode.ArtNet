namespace Haukcode.ArtNet.Rdm.Packets.Net;

/// <summary>
/// This parameter is used to retrieve the binding and control fields of a responder on an
/// endpoint (E1.37-7 §6.14). GET only.
/// </summary>
public class BindingControlFields
{
    public class Get : RdmRequestPacket
    {
        public Get()
            : base(RdmCommands.Get, RdmParameters.BindingControlFields)
        {
        }

        public ushort EndpointID { get; set; }

        /// <summary>The responder to query.</summary>
        public UId Id { get; set; } = UId.Empty;

        protected override void ReadData(RdmBinaryReader data)
        {
            EndpointID = data.ReadUInt16();
            Id = data.ReadUId();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteUInt16(EndpointID);
            data.WriteUid(Id);
        }
    }

    public class GetReply : RdmResponsePacket
    {
        public GetReply()
            : base(RdmCommands.GetResponse, RdmParameters.BindingControlFields)
        {
        }

        public ushort EndpointID { get; set; }

        public UId Id { get; set; } = UId.Empty;

        public ushort ControlFields { get; set; }

        public UId BindingId { get; set; } = UId.Empty;

        protected override void ReadData(RdmBinaryReader data)
        {
            EndpointID = data.ReadUInt16();
            Id = data.ReadUId();
            ControlFields = data.ReadUInt16();
            BindingId = data.ReadUId();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteUInt16(EndpointID);
            data.WriteUid(Id);
            data.WriteUInt16(ControlFields);
            data.WriteUid(BindingId);
        }
    }
}
