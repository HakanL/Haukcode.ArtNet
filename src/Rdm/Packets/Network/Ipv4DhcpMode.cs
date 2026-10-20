namespace Haukcode.ArtNet.Rdm.Packets.Network;

/// <summary>
/// Whether DHCP is enabled on an interface (E1.37-2 4.4). Takes effect after INTERFACE_APPLY_CONFIGURATION.
/// </summary>
public class Ipv4DhcpMode
{
    public class Get : RdmRequestPacket
    {
        public Get()
            : base(RdmCommands.Get, RdmParameters.Ipv4DhcpMode)
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
            : base(RdmCommands.GetResponse, RdmParameters.Ipv4DhcpMode)
        {
        }

        /// <summary>Interface identifier from LIST_INTERFACES.</summary>
        public uint InterfaceId { get; set; }

        public bool Enabled { get; set; }

        protected override void ReadData(RdmBinaryReader data)
        {
            InterfaceId = data.ReadUInt32();
            Enabled = data.ReadBool();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteUInt32(InterfaceId);
            data.WriteBool(Enabled);
        }
    }

    public class Set : RdmRequestPacket
    {
        public Set()
            : base(RdmCommands.Set, RdmParameters.Ipv4DhcpMode)
        {
        }

        /// <summary>Interface identifier from LIST_INTERFACES.</summary>
        public uint InterfaceId { get; set; }

        public bool Enabled { get; set; }

        protected override void ReadData(RdmBinaryReader data)
        {
            InterfaceId = data.ReadUInt32();
            Enabled = data.ReadBool();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteUInt32(InterfaceId);
            data.WriteBool(Enabled);
        }
    }

    public class SetReply : RdmResponsePacket
    {
        public SetReply()
            : base(RdmCommands.SetResponse, RdmParameters.Ipv4DhcpMode)
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
