namespace Haukcode.ArtNet.Rdm.Packets.Parameters;

/// <summary>
/// Data type of a parameter's data (E1.20 Table A-15).
/// </summary>
public enum ParameterDataTypes : byte
{
    NotDefined = 0x00,
    BitField = 0x01,
    String = 0x02,
    UInt8 = 0x03,
    Int8 = 0x04,
    UInt16 = 0x05,
    Int16 = 0x06,
    UInt32 = 0x07,
    Int32 = 0x08,
    UInt64 = 0x09,
    Int64 = 0x0A,
    Group = 0x0B,
    Uid = 0x0C,
    Boolean = 0x0D,
    Url = 0x0E,
    Mac = 0x0F,
    IPv4 = 0x10,
    IPv6 = 0x11,
    Enumeration = 0x12
}

/// <summary>
/// Command classes a parameter supports (E1.20 Table A-16).
/// </summary>
public enum ParameterCommandClasses : byte
{
    Get = 0x01,
    Set = 0x02,
    GetSet = 0x03
}
