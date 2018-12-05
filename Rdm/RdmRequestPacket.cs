using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Haukcode.Rdm
{
    public abstract class RdmRequestPacket : RdmPacket
    {
        public RdmRequestPacket(RdmCommands command, RdmParameters parameterId):base(command, parameterId)
        {
        }

        public byte PortId { get; set; }
    }
}
