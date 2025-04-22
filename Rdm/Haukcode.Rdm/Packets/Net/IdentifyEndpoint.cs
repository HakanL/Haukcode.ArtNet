using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Haukcode.Rdm.IO;

namespace Haukcode.Rdm.Packets.Net
{
    public class EndpointIdentify
    {
        public class Get:RdmRequestPacket
        {
            public Get():base(RdmCommands.Get,RdmParameters.EndpointIdentify)
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
            public GetReply()
                : base(RdmCommands.GetResponse, RdmParameters.EndpointIdentify)
            {
            }

            public short EndpointID { get; set; }

            public bool IdentifyOn { get; set; }

            protected override void ReadData(RdmBinaryReader data)
            {
                EndpointID = data.ReadInt16();
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
            public Set():base(RdmCommands.Set,RdmParameters.EndpointIdentify)
            {
            }

            public short EndpointID { get; set; }

            public bool IdentifyOn { get; set; }

            protected override void ReadData(RdmBinaryReader data)
            {
                EndpointID = data.ReadInt16();
                IdentifyOn = data.ReadBool();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteUInt16(EndpointID);
                data.WriteBool(IdentifyOn);
            }
        }

        public class SetReply : RdmRequestPacket
        {
            public SetReply()
                : base(RdmCommands.SetResponse, RdmParameters.EndpointIdentify)
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
