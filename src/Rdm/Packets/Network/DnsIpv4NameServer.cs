namespace Haukcode.ArtNet.Rdm.Packets.Network;

/// <summary>
/// One of up to three IPv4 DNS name servers (E1.37-2 4.12).
/// </summary>
public class DnsIpv4NameServer
{
    public class Get : RdmRequestPacket
    {
        public Get()
            : base(RdmCommands.Get, RdmParameters.DnsIpv4NameServer)
        {
        }

        /// <summary>Name server index, 0-2.</summary>
        public byte Index { get; set; }

        protected override void ReadData(RdmBinaryReader data)
        {
            Index = data.ReadByte();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteByte(Index);
        }
    }

    public class GetReply : RdmResponsePacket
    {
        public GetReply()
            : base(RdmCommands.GetResponse, RdmParameters.DnsIpv4NameServer)
        {
        }

        /// <summary>Name server index, 0-2.</summary>
        public byte Index { get; set; }

        /// <summary>0.0.0.0 when none is configured.</summary>
        public IPAddress Address { get; set; } = IPAddress.Any;

        protected override void ReadData(RdmBinaryReader data)
        {
            Index = data.ReadByte();
            Address = new IPAddress(data.ReadBytes(4));
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteByte(Index);
            data.WriteBytes(Address.MapToIPv4().GetAddressBytes());
        }
    }

    public class Set : RdmRequestPacket
    {
        public Set()
            : base(RdmCommands.Set, RdmParameters.DnsIpv4NameServer)
        {
        }

        /// <summary>Name server index, 0-2.</summary>
        public byte Index { get; set; }

        public IPAddress Address { get; set; } = IPAddress.Any;

        protected override void ReadData(RdmBinaryReader data)
        {
            Index = data.ReadByte();
            Address = new IPAddress(data.ReadBytes(4));
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteByte(Index);
            data.WriteBytes(Address.MapToIPv4().GetAddressBytes());
        }
    }

    public class SetReply : RdmResponsePacket
    {
        public SetReply()
            : base(RdmCommands.SetResponse, RdmParameters.DnsIpv4NameServer)
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
