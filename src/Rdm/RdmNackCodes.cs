using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Haukcode.Rdm
{
    public enum RdmNackCodes
    {
        ProxyBusy = 0xB,
        ProxyCacheExpired = 0xC,
        PortNumberInvalid = 0xD,
        UniverseTypeNotSupported = 0xE
    }
}
