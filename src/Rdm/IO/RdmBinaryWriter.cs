using System;
using System.IO;
using System.Net;
using System.Text;

namespace Haukcode.Rdm
{
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

        public void WriteByte(byte value)
        {
            this.writer.Write(value);
        }

        public void WriteString(string value)
        {
            if (!string.IsNullOrEmpty(value))
                this.writer.Write(Encoding.ASCII.GetBytes(value));
        }

        public void WriteUid(UId value)
        {
            WriteUInt16((short)value.ManufacturerId);
            WriteHiLoInt32((int)value.DeviceId);
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
}
