using Haukcode.ArtNet.IO;
using Haukcode.ArtNet.Packets;

namespace Haukcode.ArtNet.ArtNet.Packets
{
    public class ArtIpProgReplyPacket : ArtNetPacket
    {
        public byte[] IpAddress { get; set; } = { 0, 0, 0, 0 };
        public byte[] Netmask { get; set; } = { 255, 255, 255, 255 };
        public bool DhcpEnabled { get; set; } = false;
        public byte[] DefaultGateway { get; set; } = { 0, 0, 0, 0 };

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
            data.Write(new byte[4]);
            data.Write(IpAddress);
            data.Write(Netmask);
            data.Write(new byte[2]);
            data.Write((byte)(DhcpEnabled ? (1 << 6) : 0));
            data.Write(DefaultGateway);
        }
    }
}
