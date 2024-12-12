using Haukcode.ArtNet.Packets;
using Haukcode.Sockets;
using System;
using System.Net;

namespace Haukcode.Samples
{
    public class ArtTriggerSender : SampleCapture
    {
        public ArtTriggerSender(IPAddress localIp, IPAddress localSubnetMask)
            : base(localIp, localSubnetMask)
        {
        }

        protected override void Socket_NewPacket(object sender, NewPacketEventArgs<ArtNetPacket> e)
        {
            base.Socket_NewPacket(sender, e);

            switch (e.Packet.OpCode)
            {
                case ArtNet.ArtNetOpCodes.Trigger:
                    DebugPrintArtTrigger(e.Packet as ArtNet.Packets.ArtTriggerPacket);
                    break;
            }
        }

        public void SendArtTrigger(Int16 oemCode, byte key, byte subKey)
        {
            //this.socket.Send(new ArtTriggerPacket
            //{
            //    OemCode = oemCode,
            //    Key = key,
            //    SubKey = subKey,
            //    Data = new byte[512]
            //});
        }

        private void DebugPrintArtTrigger(ArtNet.Packets.ArtTriggerPacket input)
        {
            Console.WriteLine($"Trigger - OemCode: 0x{input.OemCode:X}  Key: {input.Key:X}   SubKey: {input.SubKey:X}");
        }
    }
}
