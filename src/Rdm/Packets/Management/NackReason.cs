namespace Haukcode.ArtNet.Rdm.Packets.Management;

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
    ProxyBufferFull = 0xA,

    // E1.37-2
    ActionNotSupported = 0xB,

    // E1.37-7
    EndpointNumberInvalid = 0xC,
    InvalidEndpointMode = 0xD,
    UnknownUid = 0xE,

    // E1.33
    UnknownScope = 0xF,
    InvalidStaticConfigType = 0x10,
    InvalidIPv4Address = 0x11,
    InvalidIPv6Address = 0x12,
    InvalidPort = 0x13,

    // E1.20-2025
    DeviceAbsent = 0x14,
    SensorOutOfRange = 0x15,
    SensorFault = 0x16,
    PackingNotSupported = 0x17,
    ErrorInPackedListTransaction = 0x18,
    ProxyDrop = 0x19,
    AllCallSetFail = 0x20
}
