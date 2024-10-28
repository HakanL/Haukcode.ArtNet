using System;
using Haukcode.ArtNet.IO;

namespace Haukcode.ArtNet.Packets
{
    public class ArtIpProgPacket : ArtNetPacket
    {
        private byte[] defaultGateway = { 0, 0, 0, 0 };
        private byte[] netmask = { 255, 255, 255, 255 };
        private byte[] ipAddress = { 0, 0, 0, 0 };
        public ArtIpProgCommand Command { get; set; } = ArtIpProgCommand.None;

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

        public byte[] DefaultGateway
        {
            get => defaultGateway;
            set
            {
                if (value.Length != 4)
                    throw new ArgumentException("The default gateway must be an array of 4 bytes.");

                defaultGateway = value;
            }
        }

        public ArtIpProgPacket() : base(ArtNetOpCodes.IpProg)
        {
        }

        public ArtIpProgPacket(ArtNetReceiveData data) : base(data) 
        {
        }

        public override void ReadData(ArtNetBinaryReader data) 
        {
            base.ReadData(data);
            data.ReadBytes(2);
            Command = (ArtIpProgCommand)data.ReadByte();
            data.ReadByte();
            IpAddress = data.ReadBytes(4);
            Netmask = data.ReadBytes(4);
            data.ReadBytes(2);
            DefaultGateway = data.ReadBytes(4);
        }

        public override void WriteData(ArtNetBinaryWriter data) 
        {
            base.WriteData(data);
            data.WriteByteArray(new byte[2]);
            data.WriteByte((byte)Command);
            data.WriteByteArray(new byte[1]);
            data.WriteByteArray(IpAddress);
            data.WriteByteArray(Netmask);
            data.WriteHiLoInt16(6454);
            data.WriteByteArray(DefaultGateway);
        }
    }
}
