namespace Haukcode.ArtNet.Packets;

/// <summary>
/// ArtPoll Flags (Art-Net 4, ArtPoll field 5).
/// </summary>
[Flags]
public enum ArtPollFlags : byte
{
    None = 0x00,

    /// <summary>Send ArtPollReply whenever node conditions change, not only in reply to a poll.</summary>
    ReplyOnChange = 0x02,

    /// <summary>Send me diagnostics messages.</summary>
    SendDiagnostics = 0x04,

    /// <summary>Diagnostics are unicast to this controller rather than broadcast (with <see cref="SendDiagnostics"/>).</summary>
    DiagnosticsUnicast = 0x08,

    /// <summary>Disable VLC transmission.</summary>
    DisableVlc = 0x10,

    /// <summary>Only nodes subscribed to a Port-Address in the target range reply.</summary>
    TargetedMode = 0x20
}
