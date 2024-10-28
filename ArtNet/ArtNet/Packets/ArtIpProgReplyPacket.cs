using Haukcode.ArtNet.IO;
using System;

namespace Haukcode.ArtNet.Packets
{
    public class ArtIpProgReplyPacket : ArtNetPacket
    {
        private byte[] _defaultGateway = { 0, 0, 0, 0 };
        private byte[] netmask = { 255, 255, 255, 255 };
        private byte[] ipAddress = { 0, 0, 0, 0 };

        public byte[] IpAddress
        {
            get => ipAddress;
            set
            {
                if (value.Length != 4)
                    throw new ArgumentException("The IpAddress must be an array of 4 bytes.");

                ipAddress = value;
            }
        }

        public byte[] Netmask
        {
            get => netmask;
            set
            {
                if (value.Length != 4)
                    throw new ArgumentException("The netmask must be an array of 4 bytes.");

                netmask = value;
            }
        }

        public bool DhcpEnabled { get; set; } = false;

        public byte[] DefaultGateway
        {
            get => _defaultGateway;
            set {
                if (value.Length != 4)
                    throw new ArgumentException("The default gateway must be an array of 4 bytes.");

                _defaultGateway = value;
            }
        }

        public ArtIpProgReplyPacket() : base(ArtNetOpCodes.IpProgReply) 
        {
        }

        public ArtIpProgReplyPacket(ArtNetReceiveData data) : base(data)
        {
        }

        public override void ReadData(ArtNetBinaryReader data)
        {
            base.ReadData(data);
            data.ReadBytes(4);
            IpAddress = data.ReadBytes(4);
            Netmask = data.ReadBytes(4);
            data.ReadBytes(2);
            DhcpEnabled = (data.ReadByte() & (1 << 6)) != 0;
            DefaultGateway = data.ReadBytes(4);
        }

        public override void WriteData(ArtNetBinaryWriter data)
        {
            base.WriteData(data);
            data.WriteByteArray(new byte[4]);
            data.WriteByteArray(IpAddress);
            data.WriteByteArray(Netmask);
            data.WriteByteArray(new byte[2]);
            data.WriteByte((byte)(DhcpEnabled ? (1 << 6) : 0));
            data.WriteByteArray(DefaultGateway);
        }
    }
}
