using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Haukcode.Rdm.IO;

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
                EndpointID = data.ReadInt16();
                ControlFields = data.ReadInt16();
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
}
