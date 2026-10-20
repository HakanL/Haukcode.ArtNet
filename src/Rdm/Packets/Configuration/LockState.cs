namespace Haukcode.ArtNet.Rdm.Packets.Configuration;

/// <summary>
/// This parameter is used to get or set the device's lock state (E1.37-1). State 0 is unlocked;
/// <see cref="LockStateDescription"/> describes the others.
/// </summary>
public class LockState
{
    public class Get : RdmRequestPacket
    {
        public Get()
            : base(RdmCommands.Get, RdmParameters.LockState)
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
            : base(RdmCommands.GetResponse, RdmParameters.LockState)
        {
        }

        public byte CurrentLockState { get; set; }

        public byte LockStateCount { get; set; }

        protected override void ReadData(RdmBinaryReader data)
        {
            CurrentLockState = data.ReadByte();
            LockStateCount = data.ReadByte();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteByte(CurrentLockState);
            data.WriteByte(LockStateCount);
        }
    }

    public class Set : RdmRequestPacket
    {
        public Set()
            : base(RdmCommands.Set, RdmParameters.LockState)
        {
        }

        public ushort PinCode { get; set; }

        public byte NewLockState { get; set; }

        protected override void ReadData(RdmBinaryReader data)
        {
            PinCode = data.ReadUInt16();
            NewLockState = data.ReadByte();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteUInt16(PinCode);
            data.WriteByte(NewLockState);
        }
    }

    public class SetReply : RdmResponsePacket
    {
        public SetReply()
            : base(RdmCommands.SetResponse, RdmParameters.LockState)
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
