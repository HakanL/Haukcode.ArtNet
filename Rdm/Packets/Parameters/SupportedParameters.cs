using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Haukcode.Rdm.Packets.Parameters
{
    /// <summary>
    /// The SUPPORTED PARAMETERS message is used to retrieve a packed list of supported PIDs.
    /// </summary>
    public class SupportedParameters
    {
        public class Get : RdmRequestPacket
        {
            public Get()
                : base(RdmCommands.Get,RdmParameters.SupportedParameters)
            {
            }

            protected override void ReadData(RdmBinaryReader data)
            {
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
            }
        }

        public class GetReply : RdmResponsePacket
        {
            public GetReply()
                : base(RdmCommands.GetResponse, RdmParameters.SupportedParameters)
            {
                ParameterIds = new List<RdmParameters>();
            }

            /// <summary>
            /// A list of parameter ids for parameters the device supports.
            /// </summary>
            public List<RdmParameters> ParameterIds { get; set; }

            protected override void ReadData(RdmBinaryReader data)
            {
                for (int n = 0; n < base.Header.ParameterDataLength / 2; n++)
                {
                    ParameterIds.Add((RdmParameters) data.ReadNetworkU16());
                }
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                foreach (RdmParameters parameterId in ParameterIds)
                    data.WriteNetwork((ushort) parameterId);
            }
        }
    }
}
