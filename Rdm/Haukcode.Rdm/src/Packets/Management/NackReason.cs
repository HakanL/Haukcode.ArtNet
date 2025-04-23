using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Haukcode.Rdm.Packets.Management
{
    public enum NackReason
    {
        UnknownPid = 0x0,
        FormatError = 0x1,
        HardwareFault = 0x2,
        ProxyReject = 0x3,
        WriteProtect = 0x4,
        UnsupportedCommandClass = 0x5,
        DataOutOfRange = 0x6,
        BufferFull = 0x7,
        PacketSizeUnsupported = 0x8,
        SubDeviceOutOfRange = 0x9,
        ProxyBufferFull = 0xA
    }
}
