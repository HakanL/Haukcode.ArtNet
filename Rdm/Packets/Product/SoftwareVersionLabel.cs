using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Haukcode.Rdm.Packets.Product
{
    /// <summary>
    /// This parameter is used to get a descriptive ASCII text label for the device’s operating software
    /// version. The descriptive text returned by this parameter is intended for display to the user. The
    /// label may be up to 32 characters.
    /// </summary>
    public class SoftwareVersionLabel
    {
        public class Get : RdmRequestPacket
        {
            public Get()
                : base(RdmCommands.Get,RdmParameters.SoftwareVersionLabel)
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
                : base(RdmCommands.GetResponse, RdmParameters.SoftwareVersionLabel)
            {
            }

            public string VersionLabel { get; set; }

            #region Read and Write

            protected override void ReadData(RdmBinaryReader data)
            {
                VersionLabel = data.ReadString(Header.ParameterDataLength);
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteString(VersionLabel);
            }

            #endregion
        }
    }
}
