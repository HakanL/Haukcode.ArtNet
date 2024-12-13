using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Haukcode.Rdm.Packets.Status;

namespace Haukcode.Rdm.Packets.Net
{
    public class QueuedStatusEndpointCollection
    {
        public class Set : RdmRequestPacket
        {
            public Set()
                : base(RdmCommands.Set, RdmParameters.QueuedStatusEndpointCollection)
            {
            }

            public short EndpointID { get; set; }

            public StatusTypes StatusType { get; set; }

            protected override void ReadData(RdmBinaryReader data)
            {
                EndpointID = data.ReadInt16();
                StatusType = (StatusTypes) data.ReadByte();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteUInt16(EndpointID);
                data.WriteByte((byte) StatusType);
            }
        }

        public class SetReply : RdmResponsePacket
        {
            public SetReply()
                : base(RdmCommands.SetResponse, RdmParameters.QueuedStatusEndpointCollection)
            {
            }

            protected override void ReadData(RdmBinaryReader data)
            {
                //Parameter Data Empty
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                //Parameter Data Empty
            }
        }
    }
}
