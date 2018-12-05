using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Haukcode.Rdm
{
    public enum RdmResponseTypes
    {
        Ack = 0x0,
        AckTimer = 0x1,
        NackReason = 0x2,
        AckOverflow = 0x3
    }
}
