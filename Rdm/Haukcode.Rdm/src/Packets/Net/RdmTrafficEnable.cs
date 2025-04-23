using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Haukcode.Rdm.IO;

namespace Haukcode.Rdm.Packets.Net
{
    public class RdmTrafficEnable
    {
        public class Get : RdmRequestPacket
        {
            public Get():base(RdmCommands.Get,RdmParameters.RdmTrafficEnable)
            {
            }

            public short EndpointID { get; set; }

            protected override void ReadData(RdmBinaryReader data)
            {
                EndpointID = data.ReadInt16();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteUInt16(EndpointID);
            }
        }

        public class GetReply : RdmResponsePacket
        {
            public GetReply():base(RdmCommands.GetResponse,RdmParameters.RdmTrafficEnable)
            {
            }

            public short EndpointID { get; set; }

            public bool RdmEnabled { get; set; }

            protected override void ReadData(RdmBinaryReader data)
            {
                EndpointID = data.ReadInt16();
                RdmEnabled = (data.ReadByte() > 0);
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteUInt16(EndpointID);
                data.WriteByte((byte) (RdmEnabled ? 1 : 0));
            }
        }

        public class Set : RdmRequestPacket
        {
            public Set():base(RdmCommands.Set,RdmParameters.RdmTrafficEnable)
            {
            }

            public short EndpointID { get; set; }

            public bool RdmEnabled { get; set; }

            protected override void ReadData(RdmBinaryReader data)
            {
                EndpointID = data.ReadInt16();
                RdmEnabled = (data.ReadByte() > 0);
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteUInt16(EndpointID);
                data.WriteByte((byte)(RdmEnabled ? 1 : 0));
            }
        }

        public class SetReply : RdmResponsePacket
        {
            public SetReply():base(RdmCommands.SetResponse,RdmParameters.RdmTrafficEnable)
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
