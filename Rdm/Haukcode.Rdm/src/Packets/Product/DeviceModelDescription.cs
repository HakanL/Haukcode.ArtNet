using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Haukcode.Rdm.IO;

namespace Haukcode.Rdm.Packets.Product
{
    /// <summary>
    /// This parameter provides a text description of up to 32 characters for the device model type.
    /// </summary>
    public class DeviceModelDescription
    {
        public class Get : RdmRequestPacket
        {
            public Get()
                : base(RdmCommands.Get,RdmParameters.DeviceModelDescription)
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
                : base(RdmCommands.GetResponse, RdmParameters.DeviceModelDescription)
            {
            }

            public string Description { get; set; }

            #region Read and Write

            protected override void ReadData(RdmBinaryReader data)
            {
                Description = data.ReadString(Header.ParameterDataLength);
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteBytes(Encoding.ASCII.GetBytes(Description));
            }

            #endregion
        }
    }
}
