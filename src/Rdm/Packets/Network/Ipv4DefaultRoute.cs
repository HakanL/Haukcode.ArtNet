namespace Haukcode.ArtNet.Rdm.Packets.Network;

/// <summary>
/// The device's default IPv4 route (E1.37-2 4.11). Both fields 0 (NO_DEFAULT_ROUTE) means none; a gateway sets Route, a point-to-point link sets InterfaceId.
/// </summary>
public class Ipv4DefaultRoute
{
    public class Get : RdmRequestPacket
    {
        public Get()
            : base(RdmCommands.Get, RdmParameters.Ipv4DefaultRoute)
        {
        }

        protected override void ReadData(RdmBinaryReader data)
        {
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
        }
    }

    public class GetReply : RdmResponsePacket
    {
        public GetReply()
            : base(RdmCommands.GetResponse, RdmParameters.Ipv4DefaultRoute)
        {
        }

        /// <summary>Interface used for the route, or 0.</summary>
        public uint InterfaceId { get; set; }

        /// <summary>Gateway address, or 0.0.0.0.</summary>
        public IPAddress Route { get; set; } = IPAddress.Any;

        protected override void ReadData(RdmBinaryReader data)
        {
            InterfaceId = data.ReadUInt32();
            Route = new IPAddress(data.ReadBytes(4));
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteUInt32(InterfaceId);
            data.WriteBytes(Route.MapToIPv4().GetAddressBytes());
        }
    }

    public class Set : RdmRequestPacket
    {
        public Set()
            : base(RdmCommands.Set, RdmParameters.Ipv4DefaultRoute)
        {
        }

        /// <summary>Interface used for the route, or 0.</summary>
        public uint InterfaceId { get; set; }

        /// <summary>Gateway address, or 0.0.0.0.</summary>
        public IPAddress Route { get; set; } = IPAddress.Any;

        protected override void ReadData(RdmBinaryReader data)
        {
            InterfaceId = data.ReadUInt32();
            Route = new IPAddress(data.ReadBytes(4));
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteUInt32(InterfaceId);
            data.WriteBytes(Route.MapToIPv4().GetAddressBytes());
        }
    }

    public class SetReply : RdmResponsePacket
    {
        public SetReply()
            : base(RdmCommands.SetResponse, RdmParameters.Ipv4DefaultRoute)
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
