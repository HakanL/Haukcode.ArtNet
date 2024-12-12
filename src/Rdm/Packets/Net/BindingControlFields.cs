using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Haukcode.Rdm.Packets.Net
{
    public class BindingControlFields
    {
        public class Get : RdmRequestPacket
        {
            public Get()
                : base(RdmCommands.Get, RdmParameters.BindingControlFields)
            {
            }

            public UId Id { get; set; }

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

            public UId Id { get; set; }

            public short EndpointID { get; set; }

            public short ControlFields { get; set; }

            public UId BindingId { get; set; }

            protected override void ReadData(RdmBinaryReader data)
            {
                Id = data.ReadUId();
                EndpointID = data.ReadHiLoInt16();
                ControlFields = data.ReadHiLoInt16();
                BindingId = data.ReadUId();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteUid(Id);
                data.WriteHiLoInt16(EndpointID);
                data.WriteHiLoInt16(ControlFields);
                data.WriteUid(BindingId);
            }
        }
    }
}
