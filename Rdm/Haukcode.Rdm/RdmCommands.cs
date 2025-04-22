﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Haukcode.Rdm
{
    public enum RdmCommands
    {
        Discovery = 0x10,
        DiscoveryResponse = 0x11,
        Get = 0x20,
        GetResponse = 0x21,
        Set = 0x30,
        SetResponse = 0x31
    }
}
