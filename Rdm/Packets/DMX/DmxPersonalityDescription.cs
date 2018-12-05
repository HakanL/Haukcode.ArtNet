using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Haukcode.Rdm.Packets.DMX
{
    public class DmxPersonalityDescription
    {
        public class Get : RdmRequestPacket
        {
            public Get()
                : base(RdmCommands.Get,RdmParameters.DmxPersonalityDescription)
            {
            }

            public byte PersonalityIndex { get; set; }

            #region Read and Write

            protected override void ReadData(RdmBinaryReader data)
            {
                PersonalityIndex = data.ReadByte();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.Write(PersonalityIndex);
            }

            #endregion
        }

        public class GetReply : RdmResponsePacket
        {
            public GetReply()
                : base(RdmCommands.GetResponse, RdmParameters.DmxPersonalityDescription)
            {
            }

            public byte PersonalityIndex { get; set; }

            public short DmxSlotsRequired { get; set; }

            public string Description { get; set; }
            
            #region Read and Write

            protected override void ReadData(RdmBinaryReader data)
            {
                PersonalityIndex = data.ReadByte();
                DmxSlotsRequired = data.ReadNetwork16();
                if(Header.ParameterDataLength > 3)
                    Description = data.ReadNetworkString(Header.ParameterDataLength - 3);
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.Write(PersonalityIndex);
                data.WriteNetwork(DmxSlotsRequired);
                data.WriteNetwork(Description);
            }

            #endregion
        }
    }
}
