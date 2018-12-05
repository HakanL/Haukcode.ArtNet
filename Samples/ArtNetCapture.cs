using Haukcode.ArtNet.Sockets;
using System;
using System.Net;

namespace Haukcode.Samples
{
    public class ArtNetCapture : IDisposable
    {
        private readonly ArtNetSocket socket;

        public ArtNetCapture()
        {
            this.socket = new ArtNetSocket
            {
                EnableBroadcast = true
            };

            this.socket.NewPacket += Socket_NewPacket;

            this.socket.Open(IPAddress.Any, IPAddress.Broadcast);
        }

        private void Socket_NewPacket(object sender, Sockets.NewPacketEventArgs<ArtNet.Packets.ArtNetPacket> e)
        {
            Console.WriteLine($"Received ArtNet packet with OpCode: {e.Packet.OpCode} from {e.Source}");

            switch (e.Packet.OpCode)
            {
                case ArtNet.ArtNetOpCodes.ArtTrigger:
                    DebugPrintArtTrigger(e.Packet as ArtNet.Packets.ArtTriggerPacket);
                    break;
            }
        }

        private void DebugPrintArtTrigger(ArtNet.Packets.ArtTriggerPacket input)
        {
            Console.WriteLine($"Trigger - OemCode: 0x{input.OemCode:X}  Key: {input.Key:X}   SubKey: {input.SubKey:X}");
        }

        public void Dispose()
        {
            this.socket.Close();
            this.socket.Dispose();
        }
    }
}
