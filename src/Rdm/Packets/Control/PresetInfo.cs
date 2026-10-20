namespace Haukcode.ArtNet.Rdm.Packets.Control;

/// <summary>
/// This parameter is used to retrieve the preset capabilities of a device (E1.37-1). Times are in
/// tenths of a second.
/// </summary>
public class PresetInfo
{
    public class Get : RdmRequestPacket
    {
        public Get()
            : base(RdmCommands.Get, RdmParameters.PresetInfo)
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
            : base(RdmCommands.GetResponse, RdmParameters.PresetInfo)
        {
        }

        public bool LevelFieldSupported { get; set; }

        public bool PresetSequenceSupported { get; set; }

        public bool SplitTimesSupported { get; set; }

        public bool DmxFailInfiniteDelayTimeSupported { get; set; }

        public bool DmxFailInfiniteHoldTimeSupported { get; set; }

        public bool StartupInfiniteHoldTimeSupported { get; set; }

        public ushort MaximumSceneNumber { get; set; }

        public ushort MinimumPresetFadeTime { get; set; }

        public ushort MaximumPresetFadeTime { get; set; }

        public ushort MinimumPresetWaitTime { get; set; }

        public ushort MaximumPresetWaitTime { get; set; }

        public ushort MinimumDmxFailDelayTime { get; set; }

        public ushort MaximumDmxFailDelayTime { get; set; }

        public ushort MinimumDmxFailHoldTime { get; set; }

        public ushort MaximumDmxFailHoldTime { get; set; }

        public ushort MinimumStartupDelayTime { get; set; }

        public ushort MaximumStartupDelayTime { get; set; }

        public ushort MinimumStartupHoldTime { get; set; }

        public ushort MaximumStartupHoldTime { get; set; }

        protected override void ReadData(RdmBinaryReader data)
        {
            LevelFieldSupported = data.ReadBool();
            PresetSequenceSupported = data.ReadBool();
            SplitTimesSupported = data.ReadBool();
            DmxFailInfiniteDelayTimeSupported = data.ReadBool();
            DmxFailInfiniteHoldTimeSupported = data.ReadBool();
            StartupInfiniteHoldTimeSupported = data.ReadBool();
            MaximumSceneNumber = data.ReadUInt16();
            MinimumPresetFadeTime = data.ReadUInt16();
            MaximumPresetFadeTime = data.ReadUInt16();
            MinimumPresetWaitTime = data.ReadUInt16();
            MaximumPresetWaitTime = data.ReadUInt16();
            MinimumDmxFailDelayTime = data.ReadUInt16();
            MaximumDmxFailDelayTime = data.ReadUInt16();
            MinimumDmxFailHoldTime = data.ReadUInt16();
            MaximumDmxFailHoldTime = data.ReadUInt16();
            MinimumStartupDelayTime = data.ReadUInt16();
            MaximumStartupDelayTime = data.ReadUInt16();
            MinimumStartupHoldTime = data.ReadUInt16();
            MaximumStartupHoldTime = data.ReadUInt16();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteBool(LevelFieldSupported);
            data.WriteBool(PresetSequenceSupported);
            data.WriteBool(SplitTimesSupported);
            data.WriteBool(DmxFailInfiniteDelayTimeSupported);
            data.WriteBool(DmxFailInfiniteHoldTimeSupported);
            data.WriteBool(StartupInfiniteHoldTimeSupported);
            data.WriteUInt16(MaximumSceneNumber);
            data.WriteUInt16(MinimumPresetFadeTime);
            data.WriteUInt16(MaximumPresetFadeTime);
            data.WriteUInt16(MinimumPresetWaitTime);
            data.WriteUInt16(MaximumPresetWaitTime);
            data.WriteUInt16(MinimumDmxFailDelayTime);
            data.WriteUInt16(MaximumDmxFailDelayTime);
            data.WriteUInt16(MinimumDmxFailHoldTime);
            data.WriteUInt16(MaximumDmxFailHoldTime);
            data.WriteUInt16(MinimumStartupDelayTime);
            data.WriteUInt16(MaximumStartupDelayTime);
            data.WriteUInt16(MinimumStartupHoldTime);
            data.WriteUInt16(MaximumStartupHoldTime);
        }
    }
}
