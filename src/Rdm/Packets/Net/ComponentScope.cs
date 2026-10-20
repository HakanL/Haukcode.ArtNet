namespace Haukcode.ArtNet.Rdm.Packets.Net;

/// <summary>
/// A Scope and its static Broker configuration (E1.33 7.6.2). Root device only.
/// </summary>
/// <remarks>
/// E1.33-2019 states a PDL of 0x54 for the GET response and SET, but its field list (slot 2,
/// scope 63, config type 1, IPv4 4, IPv6 16, port 2) adds up to 0x58. The fields are written as
/// listed, so these messages are 88 bytes.
/// </remarks>
public class ComponentScope
{
    /// <summary>How the Broker for a scope is found (E1.33 Table A-17).</summary>
    public enum StaticConfigTypes : byte
    {
        None = 0x00,
        IPv4 = 0x01,
        IPv6 = 0x02
    }

    public class Get : RdmRequestPacket
    {
        public Get()
            : base(RdmCommands.Get, RdmParameters.ComponentScope)
        {
        }

        /// <summary>Slot in the Scope List, from 1.</summary>
        public ushort ScopeSlot { get; set; }

        protected override void ReadData(RdmBinaryReader data)
        {
            ScopeSlot = data.ReadUInt16();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteUInt16(ScopeSlot);
        }
    }

    public class GetReply : RdmResponsePacket
    {
        public GetReply()
            : base(RdmCommands.GetResponse, RdmParameters.ComponentScope)
        {
        }

        /// <summary>Slot in the Scope List, from 1.</summary>
        public ushort ScopeSlot { get; set; }

        /// <summary>Up to 62 octets; empty removes the scope (SET).</summary>
        public string Scope { get; set; } = string.Empty;

        public StaticConfigTypes StaticConfigType { get; set; }

        public IPAddress StaticIPv4Address { get; set; } = IPAddress.Any;

        public IPAddress StaticIPv6Address { get; set; } = IPAddress.IPv6Any;

        public ushort StaticPort { get; set; }

        protected override void ReadData(RdmBinaryReader data)
        {
            ScopeSlot = data.ReadUInt16();
            Scope = TcpCommsStatus.ReadScope(data);
            StaticConfigType = (StaticConfigTypes)data.ReadByte();
            StaticIPv4Address = new IPAddress(data.ReadBytes(4));
            StaticIPv6Address = new IPAddress(data.ReadBytes(16));
            StaticPort = data.ReadUInt16();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteUInt16(ScopeSlot);
            TcpCommsStatus.WriteScope(data, Scope);
            data.WriteByte((byte)StaticConfigType);
            data.WriteBytes(StaticIPv4Address.MapToIPv4().GetAddressBytes());
            data.WriteBytes(StaticIPv6Address.MapToIPv6().GetAddressBytes());
            data.WriteUInt16(StaticPort);
        }
    }

    public class Set : RdmRequestPacket
    {
        public Set()
            : base(RdmCommands.Set, RdmParameters.ComponentScope)
        {
        }

        /// <summary>Slot in the Scope List, from 1.</summary>
        public ushort ScopeSlot { get; set; }

        /// <summary>Up to 62 octets; empty removes the scope (SET).</summary>
        public string Scope { get; set; } = string.Empty;

        public StaticConfigTypes StaticConfigType { get; set; }

        public IPAddress StaticIPv4Address { get; set; } = IPAddress.Any;

        public IPAddress StaticIPv6Address { get; set; } = IPAddress.IPv6Any;

        public ushort StaticPort { get; set; }

        protected override void ReadData(RdmBinaryReader data)
        {
            ScopeSlot = data.ReadUInt16();
            Scope = TcpCommsStatus.ReadScope(data);
            StaticConfigType = (StaticConfigTypes)data.ReadByte();
            StaticIPv4Address = new IPAddress(data.ReadBytes(4));
            StaticIPv6Address = new IPAddress(data.ReadBytes(16));
            StaticPort = data.ReadUInt16();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteUInt16(ScopeSlot);
            TcpCommsStatus.WriteScope(data, Scope);
            data.WriteByte((byte)StaticConfigType);
            data.WriteBytes(StaticIPv4Address.MapToIPv4().GetAddressBytes());
            data.WriteBytes(StaticIPv6Address.MapToIPv6().GetAddressBytes());
            data.WriteUInt16(StaticPort);
        }
    }

    public class SetReply : RdmResponsePacket
    {
        public SetReply()
            : base(RdmCommands.SetResponse, RdmParameters.ComponentScope)
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
