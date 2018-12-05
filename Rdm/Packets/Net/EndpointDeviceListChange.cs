using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Haukcode.Rdm.Packets.Net
{
    public class EndpointDeviceListChange
    {
        public class Get:RdmRequestPacket
        {
            public Get():base(RdmCommands.Get,RdmParameters.EndpointDeviceListChange)
            {
            }

            public short EndpointID { get; set; }

            protected override void ReadData(RdmBinaryReader data)
            {
                EndpointID = data.ReadNetwork16();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteNetwork(EndpointID);
            }
        }

        public class Reply:RdmResponsePacket
        {
            public Reply()
                : base(RdmCommands.GetResponse, RdmParameters.EndpointDeviceListChange)
            {             
            }

            public short EndpointID { get; set; }

            public int ListChangeNumber { get; set; }

            protected override void ReadData(RdmBinaryReader data)
            {
                EndpointID = data.ReadNetwork16();
                ListChangeNumber = data.ReadNetwork32();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteNetwork(EndpointID);
                data.WriteNetwork(ListChangeNumber);
            }
        }
    }
}
