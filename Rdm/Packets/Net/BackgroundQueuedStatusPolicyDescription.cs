using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Haukcode.Rdm.Packets.Net
{
    public class BackgroundQueuedStatusPolicyDescription
    {
        public class Get : RdmRequestPacket
        {
            public Get()
                : base(RdmCommands.Get,RdmParameters.BackgroundQueuedStatusPolicyDescription)
            {
            }

            public byte PolicyID { get; set; }

            protected override void ReadData(RdmBinaryReader data)
            {
                PolicyID = data.ReadByte();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.Write(PolicyID);
            }
        }

        public class GetReply : RdmResponsePacket
        {
            public GetReply()
                : base(RdmCommands.GetResponse, RdmParameters.BackgroundQueuedStatusPolicyDescription)
            {
            }

            public byte PolicyID { get; set; }

            public string Description { get; set; }

            protected override void ReadData(RdmBinaryReader data)
            {
                PolicyID = data.ReadByte();
                Description = data.ReadNetworkString(Header.ParameterDataLength - 1);
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.Write(PolicyID);
                data.WriteNetwork(Description);
            }
        }
    }
}
