namespace Haukcode.ArtNet.Rdm.Packets.Product;

/// <summary>
/// This parameter is used to retrieve a variety of information about the device that is normally
/// required by a controller.
/// </summary>
public class DeviceInfo
{
    /// <summary>RDM protocol version 1.0 (E1.20-2006/2010).</summary>
    public const ushort RdmProtocolVersion1 = 0x0100;

    /// <summary>RDM protocol version 2.0 (E1.20-2025).</summary>
    public const ushort RdmProtocolVersion2 = 0x0200;

    public class Get : RdmRequestPacket
    {
        public Get()
            : base(RdmCommands.Get,RdmParameters.DeviceInfo)
        {
        }

        #region Read and Write

        protected override void ReadData(RdmBinaryReader data)
        {
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
        }

        #endregion
    }

    public class GetReply : RdmResponsePacket
    {
        public GetReply()
            : base(RdmCommands.GetResponse, RdmParameters.DeviceInfo)
        {
        }

        /// <summary>
        /// The RDM version the device supports, major and minor in the high and low byte; see
        /// <see cref="RdmProtocolVersion1"/> and <see cref="RdmProtocolVersion2"/>.
        /// </summary>
        public ushort RdmProtocolVersion { get; set; } = RdmProtocolVersion2;

        public ushort DeviceModelId { get; set; }

        public ProductCategories ProductCategory { get; set; }

        public uint SoftwareVersionId { get; set; }

        public ushort DmxFootprint { get; set; }

        public byte DmxPersonality { get; set; }

        public byte DmxPersonalityCount { get; set; }

        public ushort DmxStartAddress { get; set; }

        public ushort SubDeviceCount { get; set; }

        public byte SensorCount { get; set; }

        #region Read and Write

        protected override void ReadData(RdmBinaryReader data)
        {
            RdmProtocolVersion = data.ReadUInt16();
            DeviceModelId = data.ReadUInt16();
            ProductCategory = (ProductCategories)data.ReadUInt16();
            SoftwareVersionId = data.ReadUInt32();
            DmxFootprint = data.ReadUInt16();
            DmxPersonality = data.ReadByte();
            DmxPersonalityCount = data.ReadByte();
            DmxStartAddress = data.ReadUInt16();
            SubDeviceCount = data.ReadUInt16();
            SensorCount = data.ReadByte();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteUInt16(RdmProtocolVersion);
            data.WriteUInt16(DeviceModelId);
            data.WriteUInt16((short) ProductCategory);
            data.WriteUInt32(SoftwareVersionId);
            data.WriteUInt16(DmxFootprint);
            data.WriteByte(DmxPersonality);
            data.WriteByte(DmxPersonalityCount);
            data.WriteUInt16(DmxStartAddress);
            data.WriteUInt16(SubDeviceCount);
            data.WriteByte(SensorCount);
        }

        #endregion

        public override string ToString()
        {
            return string.Format("{0}",ProductCategory.ToString());
        }
    }
}
