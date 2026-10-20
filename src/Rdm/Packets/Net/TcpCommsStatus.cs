namespace Haukcode.ArtNet.Rdm.Packets.Net;

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

        public IPAddress CurrentConnectionIP { get; set; } = null!;

        public ushort UnhealthyTCPEvents { get; set; }

        public ushort TCPConnectEvents { get; set; }

        protected override void ReadData(RdmBinaryReader data)
        {
            CurrentConnectionIP = new IPAddress(data.ReadBytes(4));
            UnhealthyTCPEvents = data.ReadUInt16();
            TCPConnectEvents = data.ReadUInt16();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteBytes(CurrentConnectionIP.GetAddressBytes());                
            data.WriteUInt16(UnhealthyTCPEvents);
            data.WriteUInt16(TCPConnectEvents);
        }
    }

    public class Set : RdmRequestPacket
    {
        public Set()
            : base(RdmCommands.Set, RdmParameters.TcpCommsStatus)
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
            : base(RdmCommands.SetResponse, RdmParameters.TcpCommsStatus)
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
