using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Haukcode.Rdm.Packets
{
    public class RdmRawPacket:RdmPacket
    {
        public RdmRawPacket()
            : base()
        {
        }

        public RdmRawPacket(RdmCommands command, RdmParameters parameterId):base(command,parameterId)
        {        
        }


        public byte[] Data { get; set; }

        #region Read and Write

        protected override void ReadData(RdmBinaryReader data)
        {
            Data = data.ReadBytes(Header.ParameterDataLength);
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteBytes(Data);
        }

        #endregion

    }
}
