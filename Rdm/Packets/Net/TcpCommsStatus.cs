using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Haukcode.Rdm.Packets.Net
{
    public class TcpCommsStatus
    {
        public class Get : RdmRequestPacket
        {
            public Get()
                : base(RdmCommands.Get, RdmParameters.TcpCommsStatus)
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

        public class GetReply : RdmResponsePacket
        {
            public GetReply()
                : base(RdmCommands.GetResponse, RdmParameters.TcpCommsStatus)
            {
            }

            public IPAddress CurrentConnectionIP { get; set; }

            public short UnhealthyTCPEvents { get; set; }

            public short TCPConnectEvents { get; set; }

            protected override void ReadData(RdmBinaryReader data)
            {
                CurrentConnectionIP = new IPAddress(data.ReadBytes(4));
                UnhealthyTCPEvents = data.ReadHiLoInt16();
                TCPConnectEvents = data.ReadHiLoInt16();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteByteArray(CurrentConnectionIP.GetAddressBytes());                
                data.WriteHiLoInt16(UnhealthyTCPEvents);
                data.WriteHiLoInt16(TCPConnectEvents);
            }
        }

        public class Set : RdmRequestPacket
        {
            public Set()
                : base(RdmCommands.Set, RdmParameters.EndpointLabel)
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

        public class SetReply : RdmResponsePacket
        {
            public SetReply()
                : base(RdmCommands.SetResponse, RdmParameters.EndpointLabel)
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
