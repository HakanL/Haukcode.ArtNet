namespace Haukcode.ArtNet.Rdm.Packets.Configuration;

/// <summary>
/// This parameter is used to retrieve or set the real-time clock in a device.
/// </summary>
public class RealTimeClock
{
    public class Get : RdmRequestPacket
    {
        public Get()
            : base(RdmCommands.Get, RdmParameters.RealTimeClock)
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
            : base(RdmCommands.GetResponse, RdmParameters.RealTimeClock)
        {
        }

        public DateTime ClockTime { get; set; }

        #region Read and Write

        protected override void ReadData(RdmBinaryReader data)
        {
            ClockTime = new DateTime(
                data.ReadInt16(),   //Year    
                data.ReadByte(),        //Month
                data.ReadByte(),        //Day
                data.ReadByte(),        //Hour
                data.ReadByte(),        //Minute
                data.ReadByte());       //Second

        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteUInt16((short)ClockTime.Year);
            data.WriteByte((byte)ClockTime.Month);
            data.WriteByte((byte)ClockTime.Day);
            data.WriteByte((byte)ClockTime.Hour);
            data.WriteByte((byte)ClockTime.Minute);
            data.WriteByte((byte)ClockTime.Second);
        }

        #endregion
    }

    public class Set : RdmRequestPacket
    {
        public Set()
            : base(RdmCommands.Set, RdmParameters.RealTimeClock)
        {
        }

        public DateTime ClockTime { get; set; }

        #region Read and Write

        protected override void ReadData(RdmBinaryReader data)
        {
            ClockTime = new DateTime(
                data.ReadInt16(),   //Year    
                data.ReadByte(),        //Month
                data.ReadByte(),        //Day
                data.ReadByte(),        //Hour
                data.ReadByte(),        //Minute
                data.ReadByte());       //Second

        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteUInt16((short)ClockTime.Year);
            data.WriteByte((byte)ClockTime.Month);
            data.WriteByte((byte)ClockTime.Day);
            data.WriteByte((byte)ClockTime.Hour);
            data.WriteByte((byte)ClockTime.Minute);
            data.WriteByte((byte)ClockTime.Second);
        }

        #endregion
    }

    public class SetReply : RdmResponsePacket
    {
        public SetReply()
            : base(RdmCommands.SetResponse, RdmParameters.RealTimeClock)
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
