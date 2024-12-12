using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Haukcode.Rdm.Packets.Control
{
    /// <summary>
    /// This parameter is used to get a descriptive ASCII text label for a given Self Test Operation. The
    /// label may be up to 32 characters.
    /// </summary>
    public class SelfTestDescription
    {
        public class Get : RdmRequestPacket
        {
            public Get()
                : base(RdmCommands.Get, RdmParameters.SelfTestDescription)
            {
            }

            public byte TestNumber { get; set; }

            #region Read and Write

            protected override void ReadData(RdmBinaryReader data)
            {
                TestNumber = data.ReadByte();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteByte(TestNumber);
            }

            #endregion
        }

        public class GetReply : RdmResponsePacket
        {
            public GetReply()
                : base(RdmCommands.GetResponse, RdmParameters.SelfTestDescription)
            {
            }

            public byte TestNumber { get; set; }

            public string Description { get; set; }

            #region Read and Write

            protected override void ReadData(RdmBinaryReader data)
            {
                TestNumber = data.ReadByte();
                Description = data.ReadString(Header.ParameterDataLength - 1);
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteByte(TestNumber);
                data.WriteString(Description);
            }

            #endregion
        }
    }
}
