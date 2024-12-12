using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Haukcode.Rdm.Packets.Power
{
    /// <summary>
    /// This parameter is used to retrieve the number of lamp hours or to set the counter in the device to
    /// a specific starting value.
    /// </summary>
    public class LampHours
    {
        public class Get : RdmRequestPacket
        {
            public Get()
                : base(RdmCommands.Get, RdmParameters.LampHours )
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
                : base(RdmCommands.GetResponse, RdmParameters.LampHours)
            {
            }

            public int LampHours { get; set; }

            #region Read and Write

            protected override void ReadData(RdmBinaryReader data)
            {
                LampHours = data.ReadHiLoInt32();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteHiLoInt32(LampHours);
            }

            #endregion
        }

        public class Set : RdmRequestPacket
        {
            public Set()
                : base(RdmCommands.Set, RdmParameters.LampHours)
            {
            }

            public int LampHours { get; set; }

            #region Read and Write

            protected override void ReadData(RdmBinaryReader data)
            {
                LampHours = data.ReadHiLoInt32();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteHiLoInt32(LampHours);
            }

            #endregion
        }

        public class SetReply : RdmResponsePacket
        {
            public SetReply()
                : base(RdmCommands.SetResponse, RdmParameters.LampHours)
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
