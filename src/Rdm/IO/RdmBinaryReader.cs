namespace Haukcode.ArtNet.Rdm.IO;

public class RdmBinaryReader
{
    private readonly BinaryReader reader;

    public RdmBinaryReader(Stream input)
    {
        this.reader = new BinaryReader(input);
    }

    public short ReadInt16()
    {
        // Read the bytes individually
        byte highByte = this.reader.ReadByte();
        byte lowByte = this.reader.ReadByte();

        // Combine the bytes in big-endian order
        return (short)((highByte << 8) | lowByte);
    }

    public ushort ReadUInt16()
    {
        return unchecked((ushort)ReadInt16());
    }

    public int ReadHiLoInt32()
    {
        // Read the bytes individually
        byte byte1 = this.reader.ReadByte(); // Most significant byte
        byte byte2 = this.reader.ReadByte();
        byte byte3 = this.reader.ReadByte();
        byte byte4 = this.reader.ReadByte(); // Least significant byte

        // Combine the bytes in big-endian order
        return (byte1 << 24) | (byte2 << 16) | (byte3 << 8) | byte4;
    }

    public uint ReadUInt32()
    {
        return unchecked((uint)ReadHiLoInt32());
    }

    /// <summary>
    /// Bytes left in the underlying stream, or null when it can't seek.
    /// </summary>
    public long? BytesLeft => this.reader.BaseStream.CanSeek
        ? this.reader.BaseStream.Length - this.reader.BaseStream.Position
        : null;

    /// <remarks>A zero or negative length (from a short or truncated reply) reads nothing.</remarks>
    public string ReadString(int length)
    {
        if (length <= 0)
            return string.Empty;

        return Encoding.ASCII.GetString(this.reader.ReadBytes(length)).TrimEnd((char)0);
    }

    public UId ReadUId()
    {
        return new UId(ReadUInt16(), ReadUInt32());
    }

    public bool ReadBool()
    {
        return this.reader.ReadBoolean();
    }

    public byte ReadByte()
    {
        return this.reader.ReadByte();
    }

    /// <remarks>A zero or negative count (from a short or truncated reply) reads nothing.</remarks>
    public byte[] ReadBytes(int count)
    {
        if (count <= 0)
            return [];

        return this.reader.ReadBytes(count);
    }
}
