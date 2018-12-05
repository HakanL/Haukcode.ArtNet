using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Haukcode.Rdm.Packets.Discovery
{
    /// <summary>
    /// This message and response are used for the device discovery process.
    /// </summary>
    /// <remarks>
    /// Discovery messages shall always be addressed to Root Devices (Sub-Device = 0).
    /// 
    /// This message shall always be sent to the ALL_DEVICES_ID UID Address, since all devices must process this message.
    /// </remarks>
    public class DiscoveryUniqueBranch
    {
        public class Request : RdmRequestPacket
        {
            public Request()
                : base(RdmCommands.Discovery, RdmParameters.DiscoveryUniqueBranch)
            {
            }

            /// <summary>
            /// The start ID for devices uncluded in this discovery.
            /// </summary>
            /// <remarks>
            /// A responder shall only respond to this message if its UID is greater than or equal to the 
            /// Lower Bound UID and less than or equal to the Upper Bound UID included in the message‘s parameter 
            /// data, and if it has not been muted through the DISC_MUTE message.
            /// </remarks>
            public UId LowerBoundId { get; set; }

            /// <summary>
            /// The end ID for devices uncluded in this discovery.
            /// </summary>
            /// <remarks>
            /// A responder shall only respond to this message if its UID is greater than or equal to the 
            /// Lower Bound UID and less than or equal to the Upper Bound UID included in the message‘s parameter 
            /// data, and if it has not been muted through the DISC_MUTE message.
            /// </remarks>
            public UId UpperBoundId { get; set; }

            #region Read and Write

            protected override void ReadData(RdmBinaryReader data)
            {
                LowerBoundId = data.ReadUId();
                UpperBoundId = data.ReadUId();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.Write(LowerBoundId);
                data.Write(UpperBoundId);
            }

            #endregion
        }

        public class Reply : RdmResponsePacket
        {
            public Reply()
                : base(RdmCommands.DiscoveryResponse, RdmParameters.DiscoveryUniqueBranch)
            {
            }

            public UId DeviceId { get; set; }

            #region Read and Write

            protected override void ReadData(RdmBinaryReader data)
            {
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
            }

            #endregion
        }
    }
}
