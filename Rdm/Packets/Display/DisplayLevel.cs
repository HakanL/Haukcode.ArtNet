using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Haukcode.Rdm.Packets.Configuration
{
    /// <summary>
    /// This parameter is used to retrieve or change the Display Intensity setting.
    /// </summary>
    public class DisplayLevel
    {
        public class Get : RdmRequestPacket
        {
            public Get()
                : base(RdmCommands.Get, RdmParameters.DisplayLevel)
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
                : base(RdmCommands.GetResponse, RdmParameters.DisplayLevel)
            {
            }

            public byte Level { get; set; }

            #region Read and Write

            protected override void ReadData(RdmBinaryReader data)
            {
                Level = data.ReadByte();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.Write(Level);
            }

            #endregion
        }

        public class Set : RdmRequestPacket
        {
            public Set()
                : base(RdmCommands.Set, RdmParameters.DisplayLevel)
            {
            }

            public byte Level { get; set; }

            #region Read and Write

            protected override void ReadData(RdmBinaryReader data)
            {
                Level = data.ReadByte();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.Write(Level);
            }

            #endregion
        }

        public class SetReply : RdmResponsePacket
        {
            public SetReply()
                : base(RdmCommands.SetResponse, RdmParameters.DisplayLevel)
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
