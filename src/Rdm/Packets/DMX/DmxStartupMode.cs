namespace Haukcode.ArtNet.Rdm.Packets.DMX;

/// <summary>
/// This parameter is used to get or set what the device does at power on when no DMX512 input is
/// present (E1.37-1). Times are in tenths of a second, 0xFFFF meaning infinite.
/// </summary>
public class DmxStartupMode
{
    public class Get : RdmRequestPacket
    {
        public Get()
            : base(RdmCommands.Get, RdmParameters.DmxStartupMode)
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
            : base(RdmCommands.GetResponse, RdmParameters.DmxStartupMode)
        {
        }

        public ushort SceneNumber { get; set; }

        public ushort StartupDelay { get; set; }

        public ushort HoldTime { get; set; }

        public byte Level { get; set; }

        protected override void ReadData(RdmBinaryReader data)
        {
            SceneNumber = data.ReadUInt16();
            StartupDelay = data.ReadUInt16();
            HoldTime = data.ReadUInt16();
            Level = data.ReadByte();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteUInt16(SceneNumber);
            data.WriteUInt16(StartupDelay);
            data.WriteUInt16(HoldTime);
            data.WriteByte(Level);
        }
    }

    public class Set : RdmRequestPacket
    {
        public Set()
            : base(RdmCommands.Set, RdmParameters.DmxStartupMode)
        {
        }

        public ushort SceneNumber { get; set; }

        public ushort StartupDelay { get; set; }

        public ushort HoldTime { get; set; }

        public byte Level { get; set; }

        protected override void ReadData(RdmBinaryReader data)
        {
            SceneNumber = data.ReadUInt16();
            StartupDelay = data.ReadUInt16();
            HoldTime = data.ReadUInt16();
            Level = data.ReadByte();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteUInt16(SceneNumber);
            data.WriteUInt16(StartupDelay);
            data.WriteUInt16(HoldTime);
            data.WriteByte(Level);
        }
    }

    public class SetReply : RdmResponsePacket
    {
        public SetReply()
            : base(RdmCommands.SetResponse, RdmParameters.DmxStartupMode)
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
