using System.IO;
using System.Net;
using System.Text;

namespace Haukcode.Rdm
{
    public class RdmBinaryWriter : BinaryWriter
    {
        public RdmBinaryWriter(Stream output)
            : base(output)
        {
        }

        public void WriteNetwork(byte value)
        {
            base.Write(value);
        }

        public void WriteNetwork(short value)
        {
            base.Write(IPAddress.HostToNetworkOrder(value));
        }

        public void WriteNetwork(ushort value)
        {
            base.Write(IPAddress.HostToNetworkOrder((short)value));
        }

        public void WriteNetwork(int value)
        {
            base.Write(IPAddress.HostToNetworkOrder(value));
        }

        public void WriteNetwork(string value)
        {
            if (!string.IsNullOrEmpty(value))
                Write(Encoding.ASCII.GetBytes(value));
        }


        public void Write(UId value)
        {
            WriteNetwork((short)value.ManufacturerId);
            WriteNetwork((int)value.DeviceId);
        }
    }
}
