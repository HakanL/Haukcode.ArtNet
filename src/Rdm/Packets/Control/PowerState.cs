namespace Haukcode.ArtNet.Rdm.Packets.Control;

/// <summary>
/// This parameter is used to retrieve or change the current device Power State. Power State
/// specifies the current operating mode of the device.
/// </summary>
public class PowerState
{
    public enum States
    {
        Off = 0x0,
        Shutdown = 0x1,
        Standby = 0x2,
        Normal = 0xffff
    }

    public class Get : RdmRequestPacket
    {
        public Get()
            : base(RdmCommands.Get, RdmParameters.PowerState)
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
            : base(RdmCommands.GetResponse, RdmParameters.PowerState)
        {
        }

        public States State { get; set; }

        #region Read and Write

        protected override void ReadData(RdmBinaryReader data)
        {
            State = (States)data.ReadByte();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteByte((byte) State);
        }

        #endregion
    }

    public class Set : RdmRequestPacket
    {
        public Set()
            : base(RdmCommands.Set, RdmParameters.PowerState)
        {
        }

        public States State { get; set; }

        #region Read and Write

        protected override void ReadData(RdmBinaryReader data)
        {
            State = (States)data.ReadByte();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteByte((byte)State);
        }

        #endregion
    }

    public class SetReply : RdmResponsePacket
    {
        public SetReply()
            : base(RdmCommands.SetResponse, RdmParameters.PowerState)
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
