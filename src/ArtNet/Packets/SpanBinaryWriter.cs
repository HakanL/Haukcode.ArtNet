using System;
using System.Text;

namespace Haukcode.ArtNet.Packets;

/// <summary>
/// Zero-allocation big-endian writer for packet serialization. The class-based
/// BigEndianBinaryWriter was allocated once per packet written — one object per ArtDmx
/// packet on the send path, 36,000/s at 600 universes / 60 Hz. This ref struct lives on the
/// stack; packets take it by reference so the position survives nested writes.
/// </summary>
public ref struct SpanBinaryWriter
{
    private readonly Span<byte> buffer;
    private int writePosition;

    public SpanBinaryWriter(Span<byte> buffer)
    {
        this.buffer = buffer;
        this.writePosition = 0;
    }

    public int BytesWritten => this.writePosition;

    public void WriteByte(byte value)
    {
        this.buffer[this.writePosition++] = value;
    }

    public void WriteInt16(short value)
    {
        this.buffer[this.writePosition++] = (byte)(value >> 8);
        this.buffer[this.writePosition++] = (byte)value;
    }

    public void WriteUInt16(ushort value)
    {
        this.buffer[this.writePosition++] = (byte)(value >> 8);
        this.buffer[this.writePosition++] = (byte)value;
    }

    public void WriteInt16Reverse(short value)
    {
        this.buffer[this.writePosition++] = (byte)value;
        this.buffer[this.writePosition++] = (byte)(value >> 8);
    }

    public void WriteUInt16Reverse(ushort value)
    {
        this.buffer[this.writePosition++] = (byte)value;
        this.buffer[this.writePosition++] = (byte)(value >> 8);
    }

    public void WriteInt32(int value)
    {
        this.buffer[this.writePosition++] = (byte)(value >> 24);
        this.buffer[this.writePosition++] = (byte)(value >> 16);
        this.buffer[this.writePosition++] = (byte)(value >> 8);
        this.buffer[this.writePosition++] = (byte)value;
    }

    public void WriteUInt32(uint value)
    {
        this.buffer[this.writePosition++] = (byte)(value >> 24);
        this.buffer[this.writePosition++] = (byte)(value >> 16);
        this.buffer[this.writePosition++] = (byte)(value >> 8);
        this.buffer[this.writePosition++] = (byte)value;
    }

    public void WriteBytes(ReadOnlySpan<byte> bytes)
    {
        bytes.CopyTo(this.buffer.Slice(this.writePosition));
        this.writePosition += bytes.Length;
    }

    public void WriteBytes(ReadOnlyMemory<byte> bytes)
    {
        WriteBytes(bytes.Span);
    }

    // Explicit array overload: with both the span and the memory overloads a byte[] argument
    // is ambiguous on net8/net9 (only C# 14's first-class span conversions resolve it)
    public void WriteBytes(byte[] bytes)
    {
        WriteBytes(bytes.AsSpan());
    }

    public void WriteZeros(int count)
    {
        this.buffer.Slice(this.writePosition, count).Clear();
        this.writePosition += count;
    }

    /// <summary>
    /// Fixed-width, null-terminated field: the string's UTF-8 bytes, truncated to leave room for the
    /// terminating NUL (without splitting a character), zero-padded.
    /// </summary>
    public void WriteString(string value, int length)
    {
        var field = this.buffer.Slice(this.writePosition, length);
        int bytesWritten = 0;

        if (!string.IsNullOrEmpty(value))
        {
            int maxBytes = length - 1;

            if (Encoding.UTF8.GetByteCount(value) <= maxBytes)
            {
                // The common case: encode straight into the packet, no intermediate array
                bytesWritten = Encoding.UTF8.GetBytes(value, field);
            }
            else
            {
                // Too long: encode to a scratch span and keep what fits, backing up to the start
                // of a character so a multi-byte sequence isn't cut in half
                Span<byte> scratch = stackalloc byte[Encoding.UTF8.GetMaxByteCount(value.Length)];
                int encoded = Encoding.UTF8.GetBytes(value, scratch);
                bytesWritten = Math.Min(encoded, maxBytes);
                while (bytesWritten > 0 && bytesWritten < encoded && (scratch[bytesWritten] & 0xC0) == 0x80)
                    bytesWritten--;

                scratch[..bytesWritten].CopyTo(field);
            }
        }

        field.Slice(bytesWritten).Clear();
        this.writePosition += length;
    }
}
