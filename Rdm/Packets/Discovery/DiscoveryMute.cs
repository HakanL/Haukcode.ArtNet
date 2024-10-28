using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Haukcode.Rdm.Packets.Discovery
{
    /// <summary>
    /// A responder port shall set its Mute flag when it receives this message containing its UID, or a broadcast address.
    /// </summary>
    public class DiscoveryMute
    {
        public class Request : RdmRequestPacket
        {
            public Request()
                : base(RdmCommands.Discovery, RdmParameters.DiscoveryMute)
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

        public class Reply : RdmResponsePacket
        {
            public Reply()
                : base(RdmCommands.DiscoveryResponse, RdmParameters.DiscoveryMute)
            {
            }

            /// <summary>
            /// Some control flags relating to the function of the device.
            /// </summary>
            /// <remarks>
            /// Managed Proxy Flag (Bit 0)
            /// The Managed Proxy Flag (Bit 0) shall be set to 1 when the responder is a Proxy device.
            /// 
            /// Sub-Device Flag (Bit 1)
            /// The Sub-Device Flag (Bit 1) shall be set to 1 when the responder supports Sub-Devices. See Section 9 for information on Sub-Devices.
            /// 
            /// Boot-Loader Flag (Bit 2)
            /// The Boot-Loader Flag (Bit 2) shall only be set to 1 when the device is incapable of normal operation until receiving a firmware upload.
            /// It is expected that when in this Boot-Loader mode the device will be capable of very limited RDM communication. The process of uploading firmware is beyond the scope of this document.
            /// 
            /// Proxied Device Flag (Bit 3)
            /// The Proxied Device Flag (Bit 3) shall only be set to 1 when a Proxy is responding to Discovery on behalf of another device. This flag indicates that the response has come from a Proxy, rather than the actual device.
            /// 
            /// Reserved bits (Bits 4-15)
            /// The Reserved bits (Bits 4-15) are reserved for future implementation and shall be set to 0.
            /// </remarks>
            public short ControlField { get; set; }

            /// <summary>
            /// The id for the primary port on the device.
            /// </summary>
            /// <remarks>
            /// The Binding UID field shall only be included when the responding device contains multiple responder ports. If the device does 
            /// contain multiple ports, then the Binding UID field shall contain the UID for the primary port on the device.
            /// 
            /// This Binding UID field allows the controller to associate multiple responder ports discovered within a single physical device.
            /// </remarks>
            public UId BindingId { get; set; }

            #region Read and Write

            protected override void ReadData(RdmBinaryReader data)
            {
                ControlField = data.ReadHiLoInt16();
                if (Header.ParameterDataLength > 2)
                    BindingId = data.ReadUId();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteHiLoInt16(ControlField);
                data.WriteUid(BindingId);
            }

            #endregion
        }
    }
}
