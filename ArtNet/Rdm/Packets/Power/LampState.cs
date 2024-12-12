using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Haukcode.Rdm.Packets.Power
{
    /// <summary>
    /// This parameter is used to retrieve or change the current operating state of the lamp.
    /// </summary>
    public class LampState
    {
        public enum LampStates
        {
            Off = 0,
            On = 1,
            Strike = 2,
            Standby = 3,
            NotPresent = 4,
            Error = 0x7F
        }

        public class Get : RdmRequestPacket
        {
            public Get()
                : base(RdmCommands.Get, RdmParameters.LampState )
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
                : base(RdmCommands.GetResponse, RdmParameters.LampState)
            {
            }

            public LampStates LampState { get; set; }

            #region Read and Write

            protected override void ReadData(RdmBinaryReader data)
            {
                LampState = (LampStates) data.ReadByte();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteByte((byte)LampState);
            }

            #endregion
        }

        public class Set : RdmRequestPacket
        {
            public Set()
                : base(RdmCommands.Set, RdmParameters.LampState)
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

        public class SetReply : RdmResponsePacket
        {
            public SetReply()
                : base(RdmCommands.SetResponse, RdmParameters.LampState)
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
