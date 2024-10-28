using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Haukcode.Rdm.Packets.Power
{
    /// <summary>
    /// This parameter is used to retrieve or set the number of hours of operation the device has been in
    /// use.
    /// </summary>
    /// <remarks>
    /// Some devices may only support the Get for this operation and not allow the
    /// device’s hours to be set.
    /// </remarks>
    public class DeviceHours
    {
        public class Get : RdmRequestPacket
        {
            public Get()
                : base(RdmCommands.Get, RdmParameters.DeviceHours)
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
                : base(RdmCommands.GetResponse, RdmParameters.DeviceHours)
            {
            }

            public int DeviceHours { get; set; }

            #region Read and Write

            protected override void ReadData(RdmBinaryReader data)
            {
                DeviceHours = data.ReadHiLoInt32();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteHiLoInt32(DeviceHours);
            }

            #endregion
        }

        public class Set : RdmRequestPacket
        {
            public Set()
                : base(RdmCommands.Set, RdmParameters.DeviceHours)
            {
            }

            public int DeviceHours { get; set; }

            #region Read and Write

            protected override void ReadData(RdmBinaryReader data)
            {
                DeviceHours = data.ReadHiLoInt32();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteHiLoInt32(DeviceHours);
            }

            #endregion
        }

        public class SetReply : RdmResponsePacket
        {
            public SetReply()
                : base(RdmCommands.SetResponse, RdmParameters.DeviceHours)
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
