namespace Haukcode.ArtNet.Rdm.Packets.Status;

public enum StatusTypes
{
    None = 0,
    GetLastMessage = 1,
    Advisory = 2,
    Warning = 3,
    Error = 4,
    AdvisoryCleared = 0x12,
    WarningCleared = 0x13,
    ErrorCleared = 0x14
}
