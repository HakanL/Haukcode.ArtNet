using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Haukcode.Rdm.Packets.Configuration
{
    /// <summary>
    /// This parameter is used to retrieve or change the Display Invert setting. Invert is often used to
    /// rotate the display image by 180 degrees.
    /// </summary>
    public class DisplayInvert
    {
        public enum DisplayInvertModes
        {
            Off = 0,
            On = 1,
            Auto = 2
        }

        public class Get : RdmRequestPacket
        {
            public Get()
                : base(RdmCommands.Get, RdmParameters.DisplayInvert )
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
                : base(RdmCommands.GetResponse, RdmParameters.DisplayInvert)
            {
            }

            public DisplayInvertModes Invert { get; set; }

            #region Read and Write

            protected override void ReadData(RdmBinaryReader data)
            {
                Invert = (DisplayInvertModes)data.ReadByte();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.Write((byte)Invert);
            }

            #endregion
        }

        public class Set : RdmRequestPacket
        {
            public Set()
                : base(RdmCommands.Set, RdmParameters.DisplayInvert)
            {
            }

            public DisplayInvertModes Invert { get; set; }

            #region Read and Write

            protected override void ReadData(RdmBinaryReader data)
            {
                Invert = (DisplayInvertModes)data.ReadByte();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.Write((byte)Invert);
            }

            #endregion
        }

        public class SetReply : RdmResponsePacket
        {
            public SetReply()
                : base(RdmCommands.SetResponse, RdmParameters.DisplayInvert)
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
