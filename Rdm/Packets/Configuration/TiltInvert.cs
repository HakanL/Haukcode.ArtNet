using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Haukcode.Rdm.Packets.Configuration
{
    /// <summary>
    /// This parameter is used to retrieve or change the Tilt Invert setting.
    /// </summary>
    public class TiltInvert
    {
        public class Get : RdmRequestPacket
        {
            public Get()
                : base(RdmCommands.Get, RdmParameters.TiltInvert)
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
                : base(RdmCommands.GetResponse, RdmParameters.TiltInvert)
            {
            }

            public bool Inverted { get; set; }

            #region Read and Write

            protected override void ReadData(RdmBinaryReader data)
            {
                Inverted = data.ReadBoolean();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.Write(Inverted);
            }

            #endregion
        }

        public class Set : RdmRequestPacket
        {
            public Set()
                : base(RdmCommands.Set, RdmParameters.TiltInvert)
            {
            }

            public bool Inverted { get; set; }

            #region Read and Write

            protected override void ReadData(RdmBinaryReader data)
            {
                Inverted = data.ReadBoolean();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.Write(Inverted);
            }

            #endregion
        }

        public class SetReply : RdmResponsePacket
        {
            public SetReply()
                : base(RdmCommands.SetResponse, RdmParameters.TiltInvert)
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
