namespace Haukcode.ArtNet.Rdm.Packets.Dimmer;

/// <summary>
/// This parameter is used to get or select the dimmer's modulation frequency setting (E1.37-1).
/// Settings are numbered from 1; <see cref="ModulationFrequencyDescription"/> describes each one.
/// </summary>
public class ModulationFrequency
{
    public class Get : RdmRequestPacket
    {
        public Get()
            : base(RdmCommands.Get, RdmParameters.ModulationFrequency)
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
            : base(RdmCommands.GetResponse, RdmParameters.ModulationFrequency)
        {
        }

        public byte CurrentSetting { get; set; }

        public byte SettingCount { get; set; }

        protected override void ReadData(RdmBinaryReader data)
        {
            CurrentSetting = data.ReadByte();
            SettingCount = data.ReadByte();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteByte(CurrentSetting);
            data.WriteByte(SettingCount);
        }
    }

    public class Set : RdmRequestPacket
    {
        public Set()
            : base(RdmCommands.Set, RdmParameters.ModulationFrequency)
        {
        }

        public byte Setting { get; set; }

        protected override void ReadData(RdmBinaryReader data)
        {
            Setting = data.ReadByte();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteByte(Setting);
        }
    }

    public class SetReply : RdmResponsePacket
    {
        public SetReply()
            : base(RdmCommands.SetResponse, RdmParameters.ModulationFrequency)
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
