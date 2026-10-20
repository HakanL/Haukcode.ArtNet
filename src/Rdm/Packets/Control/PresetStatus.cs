namespace Haukcode.ArtNet.Rdm.Packets.Control;

/// <summary>
/// This parameter is used to get or set the times stored with a preset scene (E1.37-1). Times are
/// in tenths of a second.
/// </summary>
public class PresetStatus
{
    public enum ProgrammedStates : byte
    {
        NotProgrammed = 0x00,
        Programmed = 0x01,
        ReadOnly = 0x02
    }

    public class Get : RdmRequestPacket
    {
        public Get()
            : base(RdmCommands.Get, RdmParameters.PresetStatus)
        {
        }

        public ushort SceneNumber { get; set; }

        protected override void ReadData(RdmBinaryReader data)
        {
            SceneNumber = data.ReadUInt16();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteUInt16(SceneNumber);
        }
    }

    public class GetReply : RdmResponsePacket
    {
        public GetReply()
            : base(RdmCommands.GetResponse, RdmParameters.PresetStatus)
        {
        }

        public ushort SceneNumber { get; set; }

        public ushort UpFadeTime { get; set; }

        public ushort DownFadeTime { get; set; }

        public ushort WaitTime { get; set; }

        public ProgrammedStates Programmed { get; set; }

        protected override void ReadData(RdmBinaryReader data)
        {
            SceneNumber = data.ReadUInt16();
            UpFadeTime = data.ReadUInt16();
            DownFadeTime = data.ReadUInt16();
            WaitTime = data.ReadUInt16();
            Programmed = (ProgrammedStates)data.ReadByte();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteUInt16(SceneNumber);
            data.WriteUInt16(UpFadeTime);
            data.WriteUInt16(DownFadeTime);
            data.WriteUInt16(WaitTime);
            data.WriteByte((byte)Programmed);
        }
    }

    public class Set : RdmRequestPacket
    {
        public Set()
            : base(RdmCommands.Set, RdmParameters.PresetStatus)
        {
        }

        public ushort SceneNumber { get; set; }

        public ushort UpFadeTime { get; set; }

        public ushort DownFadeTime { get; set; }

        public ushort WaitTime { get; set; }

        /// <summary>
        /// When set, the scene is cleared and the times are ignored.
        /// </summary>
        public bool ClearPreset { get; set; }

        protected override void ReadData(RdmBinaryReader data)
        {
            SceneNumber = data.ReadUInt16();
            UpFadeTime = data.ReadUInt16();
            DownFadeTime = data.ReadUInt16();
            WaitTime = data.ReadUInt16();
            ClearPreset = data.ReadBool();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteUInt16(SceneNumber);
            data.WriteUInt16(UpFadeTime);
            data.WriteUInt16(DownFadeTime);
            data.WriteUInt16(WaitTime);
            data.WriteBool(ClearPreset);
        }
    }

    public class SetReply : RdmResponsePacket
    {
        public SetReply()
            : base(RdmCommands.SetResponse, RdmParameters.PresetStatus)
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
