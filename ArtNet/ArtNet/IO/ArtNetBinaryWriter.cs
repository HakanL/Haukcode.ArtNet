using System;
using System.IO;
using System.Net;
using System.Text;

namespace Haukcode.ArtNet.IO
{
    public class ArtNetBinaryWriter
    {
        private readonly BinaryWriter writer;

        public ArtNetBinaryWriter(Stream output)
        {
            this.writer = new BinaryWriter(output);
        }

        public Stream BaseStream => this.writer.BaseStream;

        public void WriteHiLoInt16(short value)
        {
            // Split the short into two bytes
            byte highByte = (byte)((value >> 8) & 0xFF);
            byte lowByte = (byte)(value & 0xFF);

            // Write the bytes in big-endian order
            this.writer.Write(highByte);
            this.writer.Write(lowByte);
        }

        public void WriteString(string value, int length)
        {
            this.writer.Write(Encoding.ASCII.GetBytes(value.PadRight(length, (char)0x00)));
        }

        public void WriteByte(byte value)
        {
            this.writer.Write(value);
        }

        public void WriteLoHiInt16(short value)
        {
            // Low byte first
            this.writer.Write(value);
        }

        public void WriteByteArray(byte[] value)
        {
            this.writer.Write(value);
        }
    }
}
