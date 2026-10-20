namespace Haukcode.ArtNet.Rdm.Packets.Control;

/// <summary>
/// This parameter is used to recall pre-recorded Presets.
/// </summary>
public class PresetPlayback
{
    public enum PlayMode
    {
        Off = 0x0,
        Scene = 0x1,
        All = 0xffff
    }

    // 0x0000 is Off and 0xFFFF is All; every other value is a scene number
    private static PlayMode ToPlayMode(ushort value) =>
        value == (ushort)PlayMode.Off || value == (ushort)PlayMode.All ? (PlayMode)value : PlayMode.Scene;

    public class Get : RdmRequestPacket
    {
        public Get()
            : base(RdmCommands.Get, RdmParameters.PresetPlayback)
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
            : base(RdmCommands.GetResponse, RdmParameters.PresetPlayback)
        {
        }

        public PlayMode Mode { get; set; }

        public ushort SceneNumber { get; set; }

        public byte Level { get; set; }

        #region Read and Write

        protected override void ReadData(RdmBinaryReader data)
        {
            SceneNumber = data.ReadUInt16();
            Mode = ToPlayMode(SceneNumber);
            Level = data.ReadByte();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            if (Mode == PlayMode.Scene && (SceneNumber == (ushort)PlayMode.Off || SceneNumber == (ushort)PlayMode.All))
                throw new InvalidOperationException("Scene numbers are 1 to 0xFFFE; use PlayMode.Off or PlayMode.All for 0 and 0xFFFF.");

            data.WriteUInt16(Mode == PlayMode.Scene ? SceneNumber : (ushort)Mode);
            data.WriteByte(Level);
        }

        #endregion
    }

    public class Set : RdmRequestPacket
    {
        public Set()
            : base(RdmCommands.Set, RdmParameters.PresetPlayback)
        {
        }

        public PlayMode Mode { get; set; }

        public ushort SceneNumber { get; set; }

        public byte Level { get; set; }

        #region Read and Write

        protected override void ReadData(RdmBinaryReader data)
        {
            SceneNumber = data.ReadUInt16();
            Mode = ToPlayMode(SceneNumber);
            Level = data.ReadByte();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            if (Mode == PlayMode.Scene && (SceneNumber == (ushort)PlayMode.Off || SceneNumber == (ushort)PlayMode.All))
                throw new InvalidOperationException("Scene numbers are 1 to 0xFFFE; use PlayMode.Off or PlayMode.All for 0 and 0xFFFF.");

            data.WriteUInt16(Mode == PlayMode.Scene ? SceneNumber : (ushort)Mode);
            data.WriteByte(Level);
        }

        #endregion
    }

    public class SetReply : RdmResponsePacket
    {
        public SetReply()
            : base(RdmCommands.SetResponse, RdmParameters.PresetPlayback)
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
}
