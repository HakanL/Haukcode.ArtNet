namespace Haukcode.ArtNet.Rdm.Packets.Network;

/// <summary>
/// Current IPv4 address and netmask of an interface (E1.37-2 4.6). GET only.
/// </summary>
public class Ipv4CurrentAddress
{
    /// <summary>Whether the current address came from DHCP (E1.37-2 Table A-3).</summary>
    public enum DhcpStatuses : byte
    {
        Inactive = 0x00,
        Active = 0x01,
        Unknown = 0x02
    }

    public class Get : RdmRequestPacket
    {
        public Get()
            : base(RdmCommands.Get, RdmParameters.Ipv4CurrentAddress)
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
            : base(RdmCommands.GetResponse, RdmParameters.Ipv4CurrentAddress)
        {
        }

        /// <summary>Interface identifier from LIST_INTERFACES.</summary>
        public uint InterfaceId { get; set; }

        /// <summary>0.0.0.0 (IPV4_UNCONFIGURED) when the interface has no address.</summary>
        public IPAddress Address { get; set; } = IPAddress.Any;

        /// <summary>Prefix length, 0-32.</summary>
        public byte Netmask { get; set; }

        public DhcpStatuses DhcpStatus { get; set; }

        protected override void ReadData(RdmBinaryReader data)
        {
            InterfaceId = data.ReadUInt32();
            Address = new IPAddress(data.ReadBytes(4));
            Netmask = data.ReadByte();
            DhcpStatus = (DhcpStatuses)data.ReadByte();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteUInt32(InterfaceId);
            data.WriteBytes(Address.MapToIPv4().GetAddressBytes());
            data.WriteByte(Netmask);
            data.WriteByte((byte)DhcpStatus);
        }
    }
}
