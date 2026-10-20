namespace Haukcode.ArtNet.Rdm.Packets.Configuration;

/// <summary>
/// This parameter is used to get or change the PIN that locks the device (E1.37-1). PINs are 0 to 9999.
/// </summary>
public class LockPin
{
    public class Get : RdmRequestPacket
    {
        public Get()
            : base(RdmCommands.Get, RdmParameters.LockPin)
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
            : base(RdmCommands.GetResponse, RdmParameters.LockPin)
        {
        }

        public ushort PinCode { get; set; }

        protected override void ReadData(RdmBinaryReader data)
        {
            PinCode = data.ReadUInt16();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteUInt16(PinCode);
        }
    }

    public class Set : RdmRequestPacket
    {
        public Set()
            : base(RdmCommands.Set, RdmParameters.LockPin)
        {
        }

        public ushort NewPinCode { get; set; }

        public ushort CurrentPinCode { get; set; }

        protected override void ReadData(RdmBinaryReader data)
        {
            NewPinCode = data.ReadUInt16();
            CurrentPinCode = data.ReadUInt16();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteUInt16(NewPinCode);
            data.WriteUInt16(CurrentPinCode);
        }
    }

    public class SetReply : RdmResponsePacket
    {
        public SetReply()
            : base(RdmCommands.SetResponse, RdmParameters.LockPin)
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
