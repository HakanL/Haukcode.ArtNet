using System.Text;

namespace Haukcode.Rdm.IO
{
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

        public string ReadString(int length)
        {
            return Encoding.ASCII.GetString(this.reader.ReadBytes(length)).TrimEnd((char)0);
        }

        public UId ReadUId()
        {
            return new UId((ushort)ReadInt16(), (uint)ReadHiLoInt32());
        }

        public bool ReadBool()
        {
            return this.reader.ReadBoolean();
        }

        public byte ReadByte()
        {
            return this.reader.ReadByte();
        }

        public byte[] ReadBytes(int count)
        {
            return this.reader.ReadBytes(count);
        }
    }
}
