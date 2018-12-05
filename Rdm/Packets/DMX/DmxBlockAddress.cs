using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Haukcode.Rdm.Packets.DMX
{
    /// <summary>
    /// This parameter provides a mechanism for block addressing the DMX512 start address of sub-devices. 
    /// </summary>
    /// <remarks>
    /// Sub-devices implementations, such as dimmer racks, are often composed of an array of sub-devices (i.e. 
    /// dimmer modules) that allow a DMX512 start address to be set for the sub-device. Often it is desirable to 
    /// linearly address the sub-devices to consume a contiguous block of DMX512 slots. This message 
    /// provides a convenient way of accomplishing this without the need of sending a SET_COMMAND 
    /// message to address each sub-device. 
    /// </remarks>
    public class DmxBlockAddress
    {
        public class Get : RdmRequestPacket
        {
            public Get()
                : base(RdmCommands.Get, RdmParameters.DmxBlockAddress)
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
                : base(RdmCommands.GetResponse, RdmParameters.DmxBlockAddress)
            {
            }

            /// <summary>
            /// The Total Sub-Device Footprint shall return the total combined DMX512 footprint (number of consecutive 
            /// DMX512 slots required) of all the sub-devices within the device.
            /// </summary>
            /// <remarks>
            /// The footprint of the root device shall not be included within this footprint field. 
            /// </remarks>
            public short TotalDeviceFootprint { get; set; }

            /// <summary>
            /// The first DMX address of all sub-devices.
            /// </summary>
            /// <remarks>
            /// The GET_COMMAND returns the current base DMX512 start address for the array of sub-devices. This 
            /// is equivalent to the DMX512 Start Address of the first sub-device if the sub-devices are all linearly 
            /// addressed as a contiguous block. If the sub-devices are not currently linearly addressed as a contiguous 
            /// block then this field shall be set to 0xFFFF in the response message. 
            /// </remarks>
            public short DmxAddress { get; set; }

            #region Read and Write

            protected override void ReadData(RdmBinaryReader data)
            {
                TotalDeviceFootprint = data.ReadNetwork16();
                DmxAddress = data.ReadNetwork16();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteNetwork(TotalDeviceFootprint);
                data.WriteNetwork(DmxAddress);
            }

            #endregion
        }

        public class Set : RdmRequestPacket
        {
            public Set()
                : base(RdmCommands.Set, RdmParameters.DmxBlockAddress)
            {
            }

            /// <summary>
            /// The first DMX address of all sub-devices.
            /// </summary>
            /// <remarks>
            /// The SET_COMMAND shall set the DMX512 address for the first sub-device to the specified address and 
            /// the device shall automatically address each sub-device incrementally accounting for the footprint size of 
            /// each sub-device. 
            /// </remarks>
            public short DmxAddress { get; set; }

            #region Read and Write

            protected override void ReadData(RdmBinaryReader data)
            {
                DmxAddress = data.ReadNetwork16();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteNetwork(DmxAddress);
            }

            #endregion
        }

        public class SetReply : RdmResponsePacket
        {
            public SetReply()
                : base(RdmCommands.SetResponse, RdmParameters.DmxBlockAddress)
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
    }
}
