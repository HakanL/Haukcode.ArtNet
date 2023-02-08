using System;
using Haukcode.ArtNet.IO;
using Haukcode.ArtNet.Packets;

namespace Haukcode.ArtNet.ArtNet.Packets
{
    public class ArtIpProgPacket : ArtNetPacket
    {
        public ArtIpProgCommand Command { get; set; } = ArtIpProgCommand.None;
        public byte[] IpAddress { get; set; } = { 0, 0, 0, 0 };
        public byte[] Netmask { get; set; } = { 255, 255, 255, 255 };
        public byte[] DefaultGateway { get; set; } = { 0, 0, 0, 0 };

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
            data.Write(new byte[2]);
            data.Write((byte)Command);
            data.Write(new byte[1]);
            data.Write(IpAddress);
            data.Write(Netmask);
            data.Write((short)6454);
            data.Write(DefaultGateway);
        }
    }
}
