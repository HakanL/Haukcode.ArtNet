using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Haukcode.Rdm.IO;

namespace Haukcode.Rdm.Packets.Power
{
    /// <summary>
    /// This parameter is used to retrieve or change the current Lamp On Mode. Lamp On Mode defines
    /// the conditions under which a lamp will be struck.
    /// </summary>
    public class LampOnMode
    {
        /// <summary>
        /// A list of behaviours for turning the lamp on when the fixture starts.
        /// </summary>
        public enum LampOnModes
        {
            /// <summary>
            /// Lamp Stays off until directly instructed to Strike.
            /// </summary>
            Off = 0,

            /// <summary>
            /// Lamp Strikes upon receiving a DMX512 signal.
            /// </summary>
            Dmx = 1,

            /// <summary>
            /// Lamp Strikes automatically at Power-up.
            /// </summary>
            On = 2,

            /// <summary>
            /// Lamp Strikes after Calibration or Homing procedure.
            /// </summary>
            AfterCal = 3
        }

        public class Get : RdmRequestPacket
        {
            public Get()
                : base(RdmCommands.Get, RdmParameters.LampOnMode )
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
                : base(RdmCommands.GetResponse, RdmParameters.LampOnMode)
            {
            }

            public LampOnModes LampOnMode { get; set; }

            #region Read and Write

            protected override void ReadData(RdmBinaryReader data)
            {
                LampOnMode = (LampOnModes)data.ReadByte();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteByte((byte)LampOnMode);
            }

            #endregion
        }

        public class Set : RdmRequestPacket
        {
            public Set()
                : base(RdmCommands.Set, RdmParameters.LampOnMode)
            {
            }

            public LampOnModes LampOnMode { get; set; }

            #region Read and Write

            protected override void ReadData(RdmBinaryReader data)
            {
                LampOnMode = (LampOnModes)data.ReadByte();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteByte((byte)LampOnMode);
            }

            #endregion
        }

        public class SetReply : RdmResponsePacket
        {
            public SetReply()
                : base(RdmCommands.SetResponse, RdmParameters.LampOnMode)
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
