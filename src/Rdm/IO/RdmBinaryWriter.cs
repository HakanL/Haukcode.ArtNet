namespace Haukcode.ArtNet.Rdm.IO;

public class RdmBinaryWriter
{
    private readonly BinaryWriter writer;

    public RdmBinaryWriter(Stream output)
    {
        this.writer = new BinaryWriter(output);
    }

    public Stream BaseStream => this.writer.BaseStream;

    public void WriteUInt16(short value)
    {
        // Split the short into two bytes
        byte highByte = (byte)((value >> 8) & 0xFF);
        byte lowByte = (byte)(value & 0xFF);

        // Write the bytes in big-endian order
        this.writer.Write(highByte);
        this.writer.Write(lowByte);
    }

    public void WriteUInt16(ushort value)
    {
        WriteUInt16((short)value);
    }

    public void WriteHiLoInt32(int value)
    {
        // Split the int into four bytes
        byte byte1 = (byte)((value >> 24) & 0xFF); // Most significant byte
        byte byte2 = (byte)((value >> 16) & 0xFF);
        byte byte3 = (byte)((value >> 8) & 0xFF);
        byte byte4 = (byte)(value & 0xFF); // Least significant byte

        // Write the bytes in big-endian order
        this.writer.Write(byte1);
        this.writer.Write(byte2);
        this.writer.Write(byte3);
        this.writer.Write(byte4);
    }

    public void WriteUInt32(uint value)
    {
        WriteHiLoInt32(unchecked((int)value));
    }

    public void WriteByte(byte value)
    {
        this.writer.Write(value);
    }

    /// <summary>
    /// Longest RDM text field (labels and descriptions) in characters.
    /// </summary>
    public const int MaxTextLength = 32;

    /// <summary>
    /// Write text as UTF-8 instead of ASCII (E1.20-2025 10.1.1). Only set it when the controller
    /// asked for Unicode with <see cref="RdmControllerFlags.Unicode"/>; <see cref="ArtNetRdmClient"/>
    /// sets it from the request's controller flags.
    /// </summary>
    public bool AllowUnicode { get; set; }

    /// <summary>
    /// Writes <paramref name="value"/>, truncated to <see cref="MaxTextLength"/> bytes.
    /// </summary>
    public void WriteString(string value)
    {
        WriteString(value, MaxTextLength);
    }

    /// <summary>
    /// Writes <paramref name="value"/> as ASCII (or UTF-8 with <see cref="AllowUnicode"/>),
    /// truncated to <paramref name="maxLength"/> bytes, for fields longer than the usual 32
    /// (e.g. host and domain names).
    /// </summary>
    /// <remarks>
    /// UTF-8 text is truncated between whole characters (text elements), so a base character
    /// never loses its combining marks: E1.20-2025 10.1.1 says truncation shall not change the
    /// meaning of the text.
    /// </remarks>
    public void WriteString(string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value))
            return;

        if (!AllowUnicode)
        {
            var bytes = Encoding.ASCII.GetBytes(value);
            this.writer.Write(bytes, 0, Math.Min(bytes.Length, maxLength));
            return;
        }

        int length = 0;
        var elements = StringInfo.GetTextElementEnumerator(value);
        while (elements.MoveNext())
        {
            int elementLength = Encoding.UTF8.GetByteCount(elements.GetTextElement());
            if (length + elementLength > maxLength)
                break;

            length += elementLength;
        }

        var utf8 = Encoding.UTF8.GetBytes(value);
        this.writer.Write(utf8, 0, length);
    }

    public void WriteUid(UId value)
    {
        WriteUInt16(value.ManufacturerId);
        WriteUInt32(value.DeviceId);
    }

    public void WriteBool(bool value)
    {
        this.writer.Write(value);
    }

    public void WriteBytes(byte[] value)
    {
        this.writer.Write(value);
    }
}
