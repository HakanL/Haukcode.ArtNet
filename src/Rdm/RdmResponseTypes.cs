namespace Haukcode.ArtNet.Rdm;

public enum RdmResponseTypes
{
    Ack = 0x0,
    AckTimer = 0x1,
    NackReason = 0x2,
    AckOverflow = 0x3,

    /// <summary>ACK_TIMER with the delay in milliseconds (E1.20-2025).</summary>
    AckTimerHiRes = 0x4
}
