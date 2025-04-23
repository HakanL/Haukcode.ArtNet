using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Haukcode.Rdm.IO;

namespace Haukcode.Rdm.Packets.Power
{
    /// <summary>
    /// This parameter is used to retrieve the number of lamp strikes or to set the counter in the device to
    /// a specific starting value.
    /// </summary>
    public class LampStrikes
    {
        public class Get : RdmRequestPacket
        {
            public Get()
                : base(RdmCommands.Get, RdmParameters.LampStrikes )
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
                : base(RdmCommands.GetResponse, RdmParameters.LampStrikes)
            {
            }

            public int LampStrikes { get; set; }

            #region Read and Write

            protected override void ReadData(RdmBinaryReader data)
            {
                LampStrikes = data.ReadHiLoInt32();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteHiLoInt32(LampStrikes);
            }

            #endregion
        }

        public class Set : RdmRequestPacket
        {
            public Set()
                : base(RdmCommands.Set, RdmParameters.LampStrikes)
            {
            }

            public int LampStrikes { get; set; }

            #region Read and Write

            protected override void ReadData(RdmBinaryReader data)
            {
                LampStrikes = data.ReadHiLoInt32();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteHiLoInt32(LampStrikes);
            }

            #endregion
        }

        public class SetReply : RdmResponsePacket
        {
            public SetReply()
                : base(RdmCommands.SetResponse, RdmParameters.LampStrikes)
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
