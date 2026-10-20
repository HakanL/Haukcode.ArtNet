namespace Haukcode.ArtNet.Rdm.Packets.DMX;

/// <summary>
/// This parameter is used to get or set what the device does when it loses its DMX512 input
/// (E1.37-1). Times are in tenths of a second, 0xFFFF meaning infinite.
/// </summary>
public class DmxFailMode
{
    public class Get : RdmRequestPacket
    {
        public Get()
            : base(RdmCommands.Get, RdmParameters.DmxFailMode)
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
            : base(RdmCommands.GetResponse, RdmParameters.DmxFailMode)
        {
        }

        public ushort SceneNumber { get; set; }

        public ushort LossOfSignalDelay { get; set; }

        public ushort HoldTime { get; set; }

        public byte Level { get; set; }

        protected override void ReadData(RdmBinaryReader data)
        {
            SceneNumber = data.ReadUInt16();
            LossOfSignalDelay = data.ReadUInt16();
            HoldTime = data.ReadUInt16();
            Level = data.ReadByte();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteUInt16(SceneNumber);
            data.WriteUInt16(LossOfSignalDelay);
            data.WriteUInt16(HoldTime);
            data.WriteByte(Level);
        }
    }

    public class Set : RdmRequestPacket
    {
        public Set()
            : base(RdmCommands.Set, RdmParameters.DmxFailMode)
        {
        }

        public ushort SceneNumber { get; set; }

        public ushort LossOfSignalDelay { get; set; }

        public ushort HoldTime { get; set; }

        public byte Level { get; set; }

        protected override void ReadData(RdmBinaryReader data)
        {
            SceneNumber = data.ReadUInt16();
            LossOfSignalDelay = data.ReadUInt16();
            HoldTime = data.ReadUInt16();
            Level = data.ReadByte();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteUInt16(SceneNumber);
            data.WriteUInt16(LossOfSignalDelay);
            data.WriteUInt16(HoldTime);
            data.WriteByte(Level);
        }
    }

    public class SetReply : RdmResponsePacket
    {
        public SetReply()
            : base(RdmCommands.SetResponse, RdmParameters.DmxFailMode)
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
