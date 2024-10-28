using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Haukcode.Rdm.Packets.Net
{
    public class EndpointTiming
    {
        public class Get : RdmRequestPacket
        {
            public Get()
                : base(RdmCommands.Get, RdmParameters.EndpointTiming)
            {
            }

            public short EndpointID { get; set; }

            protected override void ReadData(RdmBinaryReader data)
            {
                EndpointID = data.ReadHiLoInt16();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteHiLoInt16(EndpointID);
            }
        }

        public class GetReply : RdmResponsePacket
        {
            public GetReply()
                : base(RdmCommands.GetResponse, RdmParameters.EndpointTiming)
            {
            }

            public short EndpointID { get; set; }

            public byte CurrentSetting { get; set; }

            public byte SettingsCount { get; set; }

            protected override void ReadData(RdmBinaryReader data)
            {
                EndpointID = data.ReadHiLoInt16();
                CurrentSetting = data.ReadByte();
                SettingsCount = data.ReadByte();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteHiLoInt16(EndpointID);
                data.WriteByte(CurrentSetting);
                data.WriteByte(SettingsCount);
            }
        }

        public class Set : RdmRequestPacket
        {
            public Set()
                : base(RdmCommands.Set, RdmParameters.EndpointTiming)
            {
            }

            public short EndpointID { get; set; }

            public byte PortTiming { get; set; }

            protected override void ReadData(RdmBinaryReader data)
            {
                EndpointID = data.ReadHiLoInt16();
                PortTiming = data.ReadByte();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteHiLoInt16(EndpointID);
                data.WriteByte(PortTiming);
            }
        }

        public class SetReply : RdmResponsePacket
        {
            public SetReply()
                : base(RdmCommands.SetResponse, RdmParameters.EndpointTiming)
            {
            }

            protected override void ReadData(RdmBinaryReader data)
            {
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
            }
        }
    }
}
