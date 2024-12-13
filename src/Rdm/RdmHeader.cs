using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Haukcode.Rdm
{
    public class RdmHeader
    {
        public const int Size = 8;

        public RdmHeader()
        {
            DestinationId = UId.Empty;
            SourceId = UId.Empty;
        }

        #region Packet Contents

        public byte MessageLength { get; set; }

        public UId DestinationId { get; set; }

        public UId SourceId { get; set; }

        public byte TransactionNumber { get; set; }

        public byte PortOrResponseType { get; set; }

        public byte MessageCount { get; set; }

        public short SubDevice { get; set; }

        public RdmCommands Command { get; set; }

        public RdmParameters ParameterId { get; set; }

        public byte ParameterDataLength { get; protected set;  }

        #endregion

        #region Read and Write

        public void ReadData(RdmBinaryReader data)
        {
            MessageLength = data.ReadByte();
            DestinationId = data.ReadUId();
            SourceId = data.ReadUId();
            TransactionNumber = data.ReadByte();
            PortOrResponseType = data.ReadByte();
            MessageCount = data.ReadByte();
            SubDevice = data.ReadInt16();
            Command = (RdmCommands)data.ReadByte();
            ParameterId = (RdmParameters)data.ReadInt16();
            ParameterDataLength = data.ReadByte();
        }

        private long messageLengthPosition = -1;
        private long dataLengthPosition = 0;

        public void WriteData(RdmBinaryWriter data)
        {
            // Save position so we can write length later.
            messageLengthPosition = data.BaseStream.Position;

            data.WriteByte(MessageLength);
            data.WriteUid(DestinationId);
            data.WriteUid(SourceId);
            data.WriteByte(TransactionNumber);
            data.WriteByte(PortOrResponseType);
            data.WriteByte(MessageCount);
            data.WriteUInt16(SubDevice);
            data.WriteByte((byte)Command);
            data.WriteUInt16((short)ParameterId);

            // Save position so we can write length later.
            dataLengthPosition = data.BaseStream.Position;
            data.WriteByte(ParameterDataLength);
        }

        public void WriteLength(RdmBinaryWriter data)
        {
            if (messageLengthPosition < 0 || dataLengthPosition <= 0)
                throw new InvalidOperationException("Packet data has not been written yet. You can not write the length until the body is written.");

            MessageLength = (byte)(data.BaseStream.Position - messageLengthPosition + 2);
            ParameterDataLength = (byte)((data.BaseStream.Position - 1) - dataLengthPosition);

            //Write Message Length
            data.BaseStream.Seek(messageLengthPosition, System.IO.SeekOrigin.Begin);
            data.WriteByte(MessageLength);

            //Write Parameter Data Length
            data.BaseStream.Seek(dataLengthPosition, System.IO.SeekOrigin.Begin);
            data.WriteByte(ParameterDataLength);

            //Move back to end of stream.
            data.BaseStream.Seek(0, System.IO.SeekOrigin.End);
        }
        
        #endregion
    }
}
