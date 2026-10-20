namespace Haukcode.ArtNet.Rdm;

/// <summary>
/// NACK reasons 0x0B to 0x0E, named after the published E1.37-2 / E1.37-7 codes.
/// </summary>
[Obsolete("Use Haukcode.ArtNet.Rdm.Packets.Management.NackReason, which has the full E1.20-2025 table.")]
public enum RdmNackCodes
{
    ActionNotSupported = 0xB,
    EndpointNumberInvalid = 0xC,
    InvalidEndpointMode = 0xD,
    UnknownUid = 0xE
}
