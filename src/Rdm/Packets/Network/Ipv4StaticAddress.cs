namespace Haukcode.ArtNet.Rdm.Packets.Network;

/// <summary>
/// Static IPv4 address and netmask of an interface (E1.37-2 4.7). Takes effect after INTERFACE_APPLY_CONFIGURATION.
/// </summary>
public class Ipv4StaticAddress
{
    public class Get : RdmRequestPacket
    {
        public Get()
            : base(RdmCommands.Get, RdmParameters.Ipv4StaticAddress)
        {
        }

        /// <summary>Interface identifier from LIST_INTERFACES.</summary>
        public uint InterfaceId { get; set; }

        protected override void ReadData(RdmBinaryReader data)
        {
            InterfaceId = data.ReadUInt32();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteUInt32(InterfaceId);
        }
    }

    public class GetReply : RdmResponsePacket
    {
        public GetReply()
            : base(RdmCommands.GetResponse, RdmParameters.Ipv4StaticAddress)
        {
        }

        /// <summary>Interface identifier from LIST_INTERFACES.</summary>
        public uint InterfaceId { get; set; }

        /// <summary>0.0.0.0 (IPV4_UNCONFIGURED) when none is configured.</summary>
        public IPAddress Address { get; set; } = IPAddress.Any;

        /// <summary>Prefix length, 0-32.</summary>
        public byte Netmask { get; set; }

        protected override void ReadData(RdmBinaryReader data)
        {
            InterfaceId = data.ReadUInt32();
            Address = new IPAddress(data.ReadBytes(4));
            Netmask = data.ReadByte();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteUInt32(InterfaceId);
            data.WriteBytes(Address.MapToIPv4().GetAddressBytes());
            data.WriteByte(Netmask);
        }
    }

    public class Set : RdmRequestPacket
    {
        public Set()
            : base(RdmCommands.Set, RdmParameters.Ipv4StaticAddress)
        {
        }

        /// <summary>Interface identifier from LIST_INTERFACES.</summary>
        public uint InterfaceId { get; set; }

        public IPAddress Address { get; set; } = IPAddress.Any;

        /// <summary>Prefix length, 0-32.</summary>
        public byte Netmask { get; set; }

        protected override void ReadData(RdmBinaryReader data)
        {
            InterfaceId = data.ReadUInt32();
            Address = new IPAddress(data.ReadBytes(4));
            Netmask = data.ReadByte();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteUInt32(InterfaceId);
            data.WriteBytes(Address.MapToIPv4().GetAddressBytes());
            data.WriteByte(Netmask);
        }
    }

    public class SetReply : RdmResponsePacket
    {
        public SetReply()
            : base(RdmCommands.SetResponse, RdmParameters.Ipv4StaticAddress)
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
