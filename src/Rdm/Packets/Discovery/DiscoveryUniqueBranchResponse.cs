using System.Diagnostics.CodeAnalysis;

namespace Haukcode.ArtNet.Rdm.Packets.Discovery;

/// <summary>
/// Encodes and decodes the DISC_UNIQUE_BRANCH response (E1.20 §7.5). It is not a normal RDM
/// message: there is no header, just a preamble and the responder's UID with each byte sent twice,
/// so that responses from several devices colliding on the line fail the checksum.
/// </summary>
/// <remarks>
/// Art-Net does not carry discovery traffic. ArtRdm is for non-discovery RDM messages only; the
/// node runs discovery on its own DMX line and reports the result through ArtTodRequest,
/// ArtTodControl and ArtTodData. This codec is for driving RDM on a DMX line directly.
/// </remarks>
public static class DiscoveryUniqueBranchResponse
{
    public const byte PreambleByte = 0xFE;
    public const byte PreambleSeparator = 0xAA;
    public const int MaxPreambleLength = 7;

    // Encoded UID (12 bytes) and encoded checksum (4 bytes)
    private const int EncodedLength = 16;

    /// <summary>
    /// Encodes the response a responder with <paramref name="uid"/> sends.
    /// </summary>
    /// <param name="preambleLength">Number of 0xFE preamble bytes, 0 to 7.</param>
    public static byte[] Encode(UId uid, int preambleLength = MaxPreambleLength)
    {
        if (preambleLength < 0 || preambleLength > MaxPreambleLength)
            throw new ArgumentOutOfRangeException(nameof(preambleLength), preambleLength, $"Preamble length must be 0 to {MaxPreambleLength}.");

        var result = new byte[preambleLength + 1 + EncodedLength];
        result.AsSpan(0, preambleLength).Fill(PreambleByte);
        result[preambleLength] = PreambleSeparator;

        var encoded = result.AsSpan(preambleLength + 1);
        Span<byte> uidBytes =
        [
            (byte)(uid.ManufacturerId >> 8), (byte)uid.ManufacturerId,
            (byte)(uid.DeviceId >> 24), (byte)(uid.DeviceId >> 16), (byte)(uid.DeviceId >> 8), (byte)uid.DeviceId
        ];

        ushort checksum = 0;
        for (int i = 0; i < uidBytes.Length; i++)
        {
            encoded[i * 2] = (byte)(uidBytes[i] | 0xAA);
            encoded[i * 2 + 1] = (byte)(uidBytes[i] | 0x55);
            checksum += (ushort)(encoded[i * 2] + encoded[i * 2 + 1]);
        }

        encoded[12] = (byte)((checksum >> 8) | 0xAA);
        encoded[13] = (byte)((checksum >> 8) | 0x55);
        encoded[14] = (byte)(checksum | 0xAA);
        encoded[15] = (byte)(checksum | 0x55);

        return result;
    }

    /// <summary>
    /// Decodes a response. Returns false when the data is malformed or the checksum fails, which is
    /// what a collision between several responders usually looks like; the controller should then
    /// split the UID range and try again. Some collisions still decode to a valid-looking UID, so
    /// confirm a found UID with DISC_MUTE before treating it as a device.
    /// </summary>
    public static bool TryDecode(ReadOnlySpan<byte> data, [NotNullWhen(true)] out UId? uid)
    {
        uid = null;

        int preambleLength = 0;
        while (preambleLength < data.Length && data[preambleLength] == PreambleByte)
            preambleLength++;

        if (preambleLength > MaxPreambleLength || data.Length < preambleLength + 1 + EncodedLength || data[preambleLength] != PreambleSeparator)
            return false;

        var encoded = data.Slice(preambleLength + 1, EncodedLength);

        Span<byte> decoded = stackalloc byte[8];
        ushort checksum = 0;
        for (int i = 0; i < decoded.Length; i++)
        {
            byte high = encoded[i * 2];
            byte low = encoded[i * 2 + 1];

            // Every encoded pair has these bits forced on; anything else is a collision or noise
            if ((high & 0xAA) != 0xAA || (low & 0x55) != 0x55)
                return false;

            decoded[i] = (byte)(high & low);

            if (i < 6)
                checksum += (ushort)(high + low);
        }

        if (((decoded[6] << 8) | decoded[7]) != checksum)
            return false;

        uid = new UId(
            (ushort)((decoded[0] << 8) | decoded[1]),
            (uint)((decoded[2] << 24) | (decoded[3] << 16) | (decoded[4] << 8) | decoded[5]));

        return true;
    }
}
