namespace Haukcode.ArtNet.Rdm.Packets.Network;

/// <summary>
/// Renews the DHCP lease of an interface (E1.37-2 4.9). SET only.
/// </summary>
public class InterfaceRenewDhcp
{
    public class Set : RdmRequestPacket
    {
        public Set()
            : base(RdmCommands.Set, RdmParameters.InterfaceRenewDhcp)
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

    public class SetReply : RdmResponsePacket
    {
        public SetReply()
            : base(RdmCommands.SetResponse, RdmParameters.InterfaceRenewDhcp)
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
