namespace Haukcode.ArtNet.Packets;

/// <summary>
/// NUL-terminated ASCII text in variable-length Art-Net fields (ArtDataReply, ArtDiagData,
/// ArtCommand).
/// </summary>
internal static class ArtNetText
{
    public static string Decode(ReadOnlySpan<byte> data)
    {
        int end = data.IndexOf((byte)0);

        return Encoding.ASCII.GetString(end < 0 ? data : data[..end]);
    }

    /// <summary>The text as ASCII with a NUL terminator, at most <paramref name="maxLength"/> bytes in all.</summary>
    public static byte[] Encode(string value, int maxLength)
    {
        var bytes = Encoding.ASCII.GetBytes(value ?? string.Empty);
        int length = Math.Min(bytes.Length, maxLength - 1);

        var result = new byte[length + 1];
        bytes.AsSpan(0, length).CopyTo(result);

        return result;
    }
}
