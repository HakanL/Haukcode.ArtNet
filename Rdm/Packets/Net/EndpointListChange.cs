using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Haukcode.Rdm.Packets.Net
{
    /// <summary>
    /// This parameter returns a unique ‘change number’ as a means for controllers to identify if the endpoint list has changed.
    /// </summary>
    /// <remarks>
    /// The Endpoint List Change Number is a monotonically increasing number used for controllers to track if the list of endpoints has changed. 
    /// 
    /// This Change Number shall be incremented each time the set of devices for an endpoint changes. The Change Number is an unsigned 32-bit field. This field shall be initialized to 0 and roll over from 0xFFFFFFFF to 0.
    ///
    /// Devices shall queue an ENDPOINT_LIST_CHANGE messages whenever the list of devices for an endpoint changes.
    /// </remarks>
    public class EndpointListChange
    {
        public class Get:RdmRequestPacket
        {
            public Get():base(RdmCommands.Get,RdmParameters.EndpointListChange)
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
            public Reply()
                : base(RdmCommands.GetResponse, RdmParameters.EndpointListChange)
            {             
            }

            private int listChangeNumber = 0;

            public int ListChangeNumber
            {
                get { return listChangeNumber; }
                set { listChangeNumber = value; }
            }

            protected override void ReadData(RdmBinaryReader data)
            {
                ListChangeNumber = data.ReadNetwork32();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteNetwork(ListChangeNumber);
            }
        }
    }
}
