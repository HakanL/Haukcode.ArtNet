using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Haukcode.Rdm.Packets.Net
{
    /// <summary>
    /// This parameter is used to retrieve a packed list of all endpoints that exist on an E1.33 device, with the exception of the Management Endpoint.
    /// </summary>
    /// <remarks>
    /// The list of Endpoint IDs shall not include the Management Endpoint ID. If the device does not have any Endpoints (other than the Management Endpoint) then it shall return a PDL of 0.
    /// </remarks>
    public class EndpointList
    {
        public class Get:RdmRequestPacket
        {
            public Get():base(RdmCommands.Get,RdmParameters.EndpointList)
            {
            }

            protected override void ReadData(RdmBinaryReader data)
            {
                //Parameter Data Empty
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                //Parameter Data Empty
            }
        }

        public class Reply:RdmResponsePacket
        {
            public Reply():base(RdmCommands.GetResponse,RdmParameters.EndpointList)
            {             
            }

            private int listChangeNumber = 0;

            public int ListChangeNumber
            {
                get { return listChangeNumber; }
                set { listChangeNumber = value; }
            }

            List<short> endpointIDs = new List<short>();

            public List<short> EndpointIDs 
            {
                get { return endpointIDs; }
                set { endpointIDs = value; }
            }

            protected override void ReadData(RdmBinaryReader data)
            {
                ListChangeNumber = data.ReadHiLoInt32();

                List<short> endpoints = new List<short>();
                for (int n = 0; n < ((Header.ParameterDataLength-4)/2); n++)
                {
                    endpoints.Add(data.ReadHiLoInt16());
                }

                EndpointIDs = endpoints;
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteHiLoInt32(ListChangeNumber);
                foreach (short endpointId in EndpointIDs)
                {
                    data.WriteHiLoInt16(endpointId);
                }
            }
        }
    }
}
