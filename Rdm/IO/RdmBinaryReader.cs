using System.IO;
using System.Net;
using System.Text;

namespace Haukcode.Rdm
{
    public class RdmBinaryReader : BinaryReader
    {
        public RdmBinaryReader(Stream input)
            : base(input)
        {
        }

        public short ReadNetwork16()
        {
            return (short)IPAddress.NetworkToHostOrder(ReadInt16());
        }

        public ushort ReadNetworkU16()
        {
            return (ushort)ReadNetwork16();
        }

        public int ReadNetwork32()
        {
            return (int)IPAddress.NetworkToHostOrder(ReadInt32());
        }

        public string ReadNetworkString(int length)
        {
            return Encoding.ASCII.GetString(ReadBytes(length)).TrimEnd((char)0);
        }

        public UId ReadUId()
        {
            return new UId((ushort)(int)ReadNetwork16(), (uint)ReadNetwork32());
        }
    }
}
