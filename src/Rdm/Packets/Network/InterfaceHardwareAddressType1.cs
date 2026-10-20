namespace Haukcode.ArtNet.Rdm.Packets.Network;

/// <summary>
/// EUI-48 (MAC) address of a network interface (E1.37-2 4.3). GET only.
/// </summary>
public class InterfaceHardwareAddressType1
{
    public class Get : RdmRequestPacket
    {
        public Get()
            : base(RdmCommands.Get, RdmParameters.InterfaceHardwareAddressType1)
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
            : base(RdmCommands.GetResponse, RdmParameters.InterfaceHardwareAddressType1)
        {
        }

        /// <summary>Interface identifier from LIST_INTERFACES.</summary>
        public uint InterfaceId { get; set; }

        /// <summary>EUI-48 address, big-endian.</summary>
        public byte[] HardwareAddress { get; set; } = new byte[6];

        protected override void ReadData(RdmBinaryReader data)
        {
            InterfaceId = data.ReadUInt32();
            HardwareAddress = data.ReadBytes(6);
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteUInt32(InterfaceId);
            data.WriteBytes(HardwareAddress);
        }
    }
}
