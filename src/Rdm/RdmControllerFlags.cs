namespace Haukcode.ArtNet.Rdm;

/// <summary>
/// Controller Flags, sent in the request's Message Count byte (E1.20-2025 6.2.8).
/// </summary>
[Flags]
public enum RdmControllerFlags : byte
{
    None = 0x00,

    /// <summary>The controller accepts UTF-8 text in responses.</summary>
    Unicode = 0x01,

    /// <summary>The controller understands ACK_TIMER_HI_RES.</summary>
    HiResAckTimerSupport = 0x02
}
