using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
                EndpointID = data.ReadNetwork16();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteNetwork(EndpointID);
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
                EndpointID = data.ReadNetwork16();
                RdmEnabled = (data.ReadByte() > 0);
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteNetwork(EndpointID);
                data.Write((byte) (RdmEnabled ? 1 : 0));
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
                EndpointID = data.ReadNetwork16();
                RdmEnabled = (data.ReadByte() > 0);
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteNetwork(EndpointID);
                data.Write((byte)(RdmEnabled ? 1 : 0));
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
