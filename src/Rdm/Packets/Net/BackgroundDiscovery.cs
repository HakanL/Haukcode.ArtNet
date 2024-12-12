using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Haukcode.Rdm.Packets.Net
{
    public class BackgroundDiscovery
    {
        public class Get : RdmRequestPacket
        {
            public Get()
                : base(RdmCommands.Get,RdmParameters.BackgroundDiscovery)
            {
            }

            public short EndpointID { get; set; }

            protected override void ReadData(RdmBinaryReader data)
            {
                EndpointID = data.ReadHiLoInt16();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteUInt16(EndpointID);
            }
        }

        public class GetReply : RdmResponsePacket
        {
            public GetReply()
                : base(RdmCommands.GetResponse, RdmParameters.BackgroundDiscovery)
            {
            }

            public short EndpointID { get; set; }

            /// <summary>
            /// Controls whether background discovery is enabled within the RDM device.
            /// </summary>
            public bool BackgroundDiscovery { get; set; }

            protected override void ReadData(RdmBinaryReader data)
            {
                EndpointID = data.ReadHiLoInt16();
                BackgroundDiscovery = data.ReadBool();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteUInt16(EndpointID);
                data.WriteBool(BackgroundDiscovery);
            }
        }

        public class Set : RdmRequestPacket
        {
            public Set()
                : base(RdmCommands.Set, RdmParameters.BackgroundDiscovery)
            {
            }

            public short EndpointID { get; set; }

            /// <summary>
            /// Controls whether background discovery is enabled within the RDM device.
            /// </summary>
            public bool BackgroundDiscovery { get; set; }

            protected override void ReadData(RdmBinaryReader data)
            {
                EndpointID = data.ReadHiLoInt16();
                BackgroundDiscovery = data.ReadBool();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteUInt16(EndpointID);
                data.WriteBool(BackgroundDiscovery);
            }
        }

        public class SetReply : RdmResponsePacket
        {
            public SetReply()
                : base(RdmCommands.SetResponse, RdmParameters.BackgroundDiscovery)
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
