using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Haukcode.Rdm.Packets.Status;

namespace Haukcode.Rdm.Packets.Net
{
    public class QueuedStatusUIDCollection
    {
        public class Set : RdmRequestPacket
        {
            public Set()
                : base(RdmCommands.Set, RdmParameters.QueuedStatusUIDCollection)
            {
            }

            public short EndpointID { get; set; }

            public UId TargetUID { get; set; }

            public StatusTypes StatusType { get; set; }

            protected override void ReadData(RdmBinaryReader data)
            {
                EndpointID = data.ReadNetwork16();
                TargetUID = data.ReadUId();
                StatusType = (StatusTypes) data.ReadByte();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteNetwork(EndpointID);
                data.Write(TargetUID);
                data.Write((byte) StatusType);
            }
        }

        public class SetReply : RdmResponsePacket
        {
            public SetReply()
                : base(RdmCommands.SetResponse, RdmParameters.QueuedStatusUIDCollection)
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
