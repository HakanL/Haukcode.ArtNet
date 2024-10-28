using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Haukcode.Rdm.Packets.Management
{
    public class RdmAckTimer : RdmPacket
    {
        public RdmAckTimer()
        {
        }

        public RdmAckTimer(RdmCommands command, RdmParameters parameterId)
            : base(command, parameterId)
        {
            Header.PortOrResponseType = (byte)RdmResponseTypes.AckTimer;
        }

        public short EstimatedResponseTime { get; set; }

        #region Read and Write

        protected override void ReadData(RdmBinaryReader data)
        {
            EstimatedResponseTime = data.ReadHiLoInt16();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteHiLoInt16(EstimatedResponseTime);
        }

        #endregion

    }
}
