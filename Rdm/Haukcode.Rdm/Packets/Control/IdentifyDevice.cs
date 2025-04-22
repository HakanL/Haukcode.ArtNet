using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Haukcode.Rdm.IO;

namespace Haukcode.Rdm.Packets.Control
{
    /// <summary>
    /// This parameter is used for the user to physically identify the device represented by the UID.
    /// </summary>
    /// <remarks>
    /// The responder shall physically identify itself using a visible or audible action. For example,
    /// strobing a light or outputting fog.
    /// </remarks>
    public class IdentifyDevice
    {
        public class Get : RdmRequestPacket
        {
            public Get()
                : base(RdmCommands.Get, RdmParameters.IdentifyDevice)
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
                : base(RdmCommands.GetResponse, RdmParameters.IdentifyDevice)
            {
            }

            public bool IdentifyEnabled { get; set; }

            #region Read and Write

            protected override void ReadData(RdmBinaryReader data)
            {
                IdentifyEnabled = data.ReadBool();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteBool(IdentifyEnabled);
            }

            #endregion
        }

        public class Set : RdmRequestPacket
        {
            public Set()
                : base(RdmCommands.Set, RdmParameters.IdentifyDevice)
            {
            }

            public bool IdentifyEnabled { get; set; }

            #region Read and Write

            protected override void ReadData(RdmBinaryReader data)
            {
                IdentifyEnabled = data.ReadBool();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteBool(IdentifyEnabled);
            }

            #endregion
        }

        public class SetReply : RdmResponsePacket
        {
            public SetReply()
                : base(RdmCommands.SetResponse, RdmParameters.IdentifyDevice)
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
