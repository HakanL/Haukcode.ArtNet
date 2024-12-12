using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Haukcode.Rdm.Packets.Product
{
    /// <summary>
    /// This parameter provides an ASCII text response with the Manufacturer name for the device of up
    /// to 32 characters. The Manufacturer name must be consistent between all products manufactured
    /// within an ESTA Manufacturer ID.
    /// </summary>
    public class ManufacturerLabel
    {
        public class Get : RdmRequestPacket
        {
            public Get()
                : base(RdmCommands.Get,RdmParameters.ManufacturerLabel)
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
                : base(RdmCommands.GetResponse, RdmParameters.ManufacturerLabel)
            {
            }

            public string Label { get; set; }

            #region Read and Write
            
            protected override void ReadData(RdmBinaryReader data)
            {
                Label = data.ReadString(Header.ParameterDataLength);
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteBytes(Encoding.ASCII.GetBytes(Label));
            }

            #endregion

        }
    }
}
