namespace Haukcode.ArtNet.Rdm.Packets.Product;

/// <summary>
/// RDM language codes are 2-character ISO 639-1 codes. They're kept as strings so an unknown code
/// (or running with invariant globalization) never fails a parse; <see cref="ToCulture"/> gives
/// the <see cref="CultureInfo"/> when there is one.
/// </summary>
public static class LanguageCodes
{
    public const int Length = 2;

    /// <summary>
    /// The culture for <paramref name="code"/>, or null when it isn't a known culture.
    /// </summary>
    public static CultureInfo? ToCulture(string? code)
    {
        if (string.IsNullOrWhiteSpace(code))
            return null;

        try
        {
            return CultureInfo.GetCultureInfo(code);
        }
        catch (CultureNotFoundException)
        {
            return null;
        }
    }

    /// <summary>
    /// The 2-letter code for <paramref name="culture"/>, or "" when it has none (some cultures only
    /// have a 3-letter ISO 639-2 code).
    /// </summary>
    public static string FromCulture(CultureInfo? culture)
    {
        var code = culture?.TwoLetterISOLanguageName;

        return code?.Length == Length ? code : string.Empty;
    }

    internal static string Read(RdmBinaryReader data) => data.ReadString(Length);

    /// <summary>Writes exactly 2 bytes, zero-padded or truncated.</summary>
    internal static void Write(RdmBinaryWriter data, string code)
    {
        var bytes = Encoding.ASCII.GetBytes(code ?? string.Empty);
        for (int i = 0; i < Length; i++)
            data.WriteByte(i < bytes.Length ? bytes[i] : (byte)0);
    }
}
