using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Haukcode.Rdm.Packets.Control
{
    /// <summary>
    /// This parameter is used to instruct the responder to reset itself. This parameter shall also clear the
    /// Discovery Mute flag. A cold reset is the equivalent of removing and reapplying power to the
    /// device.
    /// </summary>
    public class ResetDevice
    {
        public enum ResetType
        {
            WarmReset = 0x1,
            ColdReset = 0xFF
        }

        public class Set : RdmRequestPacket
        {
            public Set()
                : base(RdmCommands.Set, RdmParameters.ResetDevice)
            {
            }

            public ResetType Reset { get; set; }

            #region Read and Write

            protected override void ReadData(RdmBinaryReader data)
            {
                Reset = (ResetType)data.ReadByte();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.Write((byte)Reset);
            }

            #endregion
        }

        public class SetReply : RdmResponsePacket
        {
            public SetReply()
                : base(RdmCommands.SetResponse, RdmParameters.ResetDevice)
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
