using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Haukcode.Rdm.Packets.Control
{
    /// <summary>
    /// Different modes for identifying fixtures.
    /// </summary>
    public enum IdentifyModes
    {
        Quit = 0x0,
        Loud = 0xFF
    }

    /// <summary>
    /// This parameter is used to get or set the RDM Identify Mode. 
    /// </summary>
    /// <remarks>
    /// This parameter allows devices to have different Identify Modes for use with the IDENTIFY_DEVICE 
    /// message. 
    /// </remarks>
    public class IdentifyMode
    {
        public class Get : RdmRequestPacket
        {
            public Get()
                : base(RdmCommands.Get, RdmParameters.IdentifyMode)
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
                : base(RdmCommands.GetResponse, RdmParameters.IdentifyMode)
            {
            }

            public IdentifyModes IdentifyMode { get; set; }

            #region Read and Write

            protected override void ReadData(RdmBinaryReader data)
            {
                IdentifyMode = (IdentifyModes) data.ReadByte();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteByte((byte) IdentifyMode);
            }

            #endregion
        }

        public class Set : RdmRequestPacket
        {
            public Set()
                : base(RdmCommands.Set, RdmParameters.IdentifyMode)
            {
            }

            public IdentifyModes IdentifyMode { get; set; }

            #region Read and Write

            protected override void ReadData(RdmBinaryReader data)
            {
                IdentifyMode = (IdentifyModes) data.ReadByte();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteByte((byte) IdentifyMode);
            }

            #endregion
        }

        public class SetReply : RdmResponsePacket
        {
            public SetReply()
                : base(RdmCommands.SetResponse, RdmParameters.IdentifyMode)
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
