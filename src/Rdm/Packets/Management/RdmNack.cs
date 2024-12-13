using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Haukcode.Rdm.Packets.Management
{
    public class RdmNack:RdmPacket
    {
        public RdmNack()
        {
        }

        public RdmNack(RdmCommands command, RdmParameters parameterId)
            : base(command, parameterId)
        {
            Header.PortOrResponseType = (byte) RdmResponseTypes.NackReason;
        }

        public NackReason Reason { get; set; }

        #region Read and Write

        protected override void ReadData(RdmBinaryReader data)
        {
            Reason = (NackReason) data.ReadInt16();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteByte((byte) Reason);
        }

        #endregion

    }
}
