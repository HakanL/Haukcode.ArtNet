using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Haukcode.Rdm.Packets.Status
{
    public enum StatusTypes
    {
        None = 0,
        GetLastMessage = 1,
        Advisory = 2,
        Warning = 3,
        Error = 4
    }
}
