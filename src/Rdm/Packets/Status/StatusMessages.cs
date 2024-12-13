using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Haukcode.Rdm.Packets.Status
{
    /// <summary>
    /// This parameter is used to collect Status or Error information from a device.
    /// </summary>
    /// <remarks>
    /// The Status Type of STATUS_NONE shall be used when a controller wants to establish whether a
    /// device is present on the network without retrieving any Status Message data from the device.
    /// </remarks>
    public class StatusMessage
    {
        public struct Status
        {
            /// <summary>
            /// In a system containing sub-devices, the Sub-Device field shall be used to indicate the sub-device
            /// to which the status message belongs. If the status message does not reference a particular subdevice
            /// the field shall be set to 0x0000, to reference the root device.
            /// </summary>
            public short SubDeviceId { get; set; }

            /// <summary>
            /// The Status Type is used to identify the severity of the condition.
            /// </summary>
            public StatusTypes StatusType { get; set; }

            /// <summary>
            /// Status Message ID’s within the range of 0x0000 — 0x7FFF are reserved for publicly defined Status Messages.
            /// </summary>
            public short StatusMessageId { get; set; }

            /// <summary>
            /// Each Status Message supports the return of two separate data values relevant to the context of
            /// the specific message. The data value for ESTA public status messages is used to identify a
            /// property within the device to which the message corresponds.
            /// </summary>
            /// <remarks>
            /// Status ID’s not using the Data Value fields shall set the fields with 0x0000.
            /// </remarks>
            public short DataValue1 { get; set; }

            /// <summary>
            /// Each Status Message supports the return of two separate data values relevant to the context of
            /// the specific message. The data value for ESTA public status messages is used to identify a
            /// property within the device to which the message corresponds.
            /// </summary>
            /// <remarks>
            /// Status ID’s not using the Data Value fields shall set the fields with 0x0000.
            /// </remarks>
            public short DataValue2 { get; set; }
        }

        public class Get : RdmRequestPacket
        {
            public Get()
                : base(RdmCommands.Get, RdmParameters.StatusMessage)
            {
            }

            /// <summary>
            /// Requests the retransmission of the last sent Status Message or Queued Message.
            /// </summary>
            public StatusTypes StatusType { get; set; }

            #region Read and Write

            protected override void ReadData(RdmBinaryReader data)
            {
                StatusType = (StatusTypes)data.ReadByte(); ;
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteByte((byte)StatusType);
            }

            #endregion
        }

        public class GetReply : RdmResponsePacket
        {
            public GetReply()
                : base(RdmCommands.GetResponse, RdmParameters.StatusMessage)
            {
                StatusMessages = new List<Status>();
            }

            public List<Status> StatusMessages { get; set; }


            #region Read and Write

            protected override void ReadData(RdmBinaryReader data)
            {
                for (int n = 0; n < (Header.ParameterDataLength / 9); n++)
                {
                    Status subDeviceStatus = new Status();
                    subDeviceStatus.SubDeviceId = data.ReadInt16();
                    subDeviceStatus.StatusType = (StatusTypes) data.ReadByte();
                    subDeviceStatus.StatusMessageId = data.ReadInt16();
                    subDeviceStatus.DataValue1 = data.ReadInt16();
                    subDeviceStatus.DataValue2 = data.ReadInt16();

                    StatusMessages.Add(subDeviceStatus);
                }
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                if (StatusMessages.Count > 25)
                    throw new InvalidPacketException("A maximum of 25 status messages is allowed.");

                foreach (Status item in StatusMessages)
                {
                    data.WriteUInt16(item.SubDeviceId);
                    data.WriteByte((byte) item.StatusType);
                    data.WriteUInt16(item.StatusMessageId);
                    data.WriteUInt16(item.DataValue1);
                    data.WriteUInt16(item.DataValue2);
                }
            }

            #endregion
        }
    }
}
