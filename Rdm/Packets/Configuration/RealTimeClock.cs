using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Haukcode.Rdm.Packets.Configuration
{
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
                int year = data.ReadNetwork16();

                ClockTime = new DateTime(
                    data.ReadNetwork16(),   //Year    
                    data.ReadByte(),        //Month
                    data.ReadByte(),        //Day
                    data.ReadByte(),        //Hour
                    data.ReadByte(),        //Minute
                    data.ReadByte());       //Second

            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.Write(ClockTime.Year);
                data.Write(ClockTime.Month);
                data.Write(ClockTime.Day);
                data.Write(ClockTime.Hour);
                data.Write(ClockTime.Minute);
                data.Write(ClockTime.Second);
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
                int year = data.ReadNetwork16();

                ClockTime = new DateTime(
                    data.ReadNetwork16(),   //Year    
                    data.ReadByte(),        //Month
                    data.ReadByte(),        //Day
                    data.ReadByte(),        //Hour
                    data.ReadByte(),        //Minute
                    data.ReadByte());       //Second

            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.Write(ClockTime.Year);
                data.Write(ClockTime.Month);
                data.Write(ClockTime.Day);
                data.Write(ClockTime.Hour);
                data.Write(ClockTime.Minute);
                data.Write(ClockTime.Second);
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
}
