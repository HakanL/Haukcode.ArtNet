using System;
using System.IO;
using System.Net;
using System.Text;

namespace Haukcode.ArtNet.IO
{
    public class ArtNetBinaryReader
    {
        private readonly BinaryReader reader;

        public ArtNetBinaryReader(Stream input)
        {
            this.reader = new BinaryReader(input);
        }

        public Stream BaseStream => this.reader.BaseStream;

        public short ReadInt16()
        {
            // Read the bytes individually
            byte highByte = this.reader.ReadByte();
            byte lowByte = this.reader.ReadByte();

            // Combine the bytes in big-endian order
            return (short)((highByte << 8) | lowByte);
        }

        public string ReadString(int length)
        {
            return Encoding.ASCII.GetString(this.reader.ReadBytes(length));
        }

        public byte ReadByte()
        {
            return this.reader.ReadByte();
        }

        public byte[] ReadBytes(int count)
        {
            return this.reader.ReadBytes(count);
        }

        public short ReadInt16Reverse()
        {
            return this.reader.ReadInt16();
        }
    }
}
