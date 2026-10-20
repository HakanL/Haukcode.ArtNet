namespace Haukcode.ArtNet.Rdm.Packets.Dimmer;

/// <summary>
/// This parameter is used to retrieve the dimmer capabilities of a device (E1.37-1). GET only.
/// </summary>
public class DimmerInfo
{
    public class Get : RdmRequestPacket
    {
        public Get()
            : base(RdmCommands.Get, RdmParameters.DimmerInfo)
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
            : base(RdmCommands.GetResponse, RdmParameters.DimmerInfo)
        {
        }

        public ushort MinimumLevelLowerLimit { get; set; }

        public ushort MinimumLevelUpperLimit { get; set; }

        public ushort MaximumLevelLowerLimit { get; set; }

        public ushort MaximumLevelUpperLimit { get; set; }

        public byte NumberOfSupportedCurves { get; set; }

        /// <summary>
        /// Resolution of the levels in bits, 0x01 to 0x10.
        /// </summary>
        public byte LevelsResolution { get; set; }

        public bool MinimumLevelSplitLevelsSupported { get; set; }

        protected override void ReadData(RdmBinaryReader data)
        {
            MinimumLevelLowerLimit = data.ReadUInt16();
            MinimumLevelUpperLimit = data.ReadUInt16();
            MaximumLevelLowerLimit = data.ReadUInt16();
            MaximumLevelUpperLimit = data.ReadUInt16();
            NumberOfSupportedCurves = data.ReadByte();
            LevelsResolution = data.ReadByte();
            MinimumLevelSplitLevelsSupported = data.ReadBool();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteUInt16(MinimumLevelLowerLimit);
            data.WriteUInt16(MinimumLevelUpperLimit);
            data.WriteUInt16(MaximumLevelLowerLimit);
            data.WriteUInt16(MaximumLevelUpperLimit);
            data.WriteByte(NumberOfSupportedCurves);
            data.WriteByte(LevelsResolution);
            data.WriteBool(MinimumLevelSplitLevelsSupported);
        }
    }
}
