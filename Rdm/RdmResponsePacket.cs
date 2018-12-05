using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Haukcode.Rdm
{
    public abstract class RdmResponsePacket:RdmPacket
    {
        public RdmResponsePacket(RdmCommands command, RdmParameters parameterId):base(command, parameterId)
        {
        }

        [Browsable(false)]
        public RdmResponseTypes ResponseType { get; set; }
    }
}
