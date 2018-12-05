using Haukcode.ArtNet.Sockets;
using System;
using System.Net;

namespace Haukcode.Samples
{
    public abstract class SampleBase : IDisposable
    {
        protected readonly ArtNetSocket socket;

        public SampleBase(IPAddress localIp, IPAddress localSubnetMask)
        {
            this.socket = new ArtNetSocket
            {
                EnableBroadcast = true
            };

            this.socket.NewPacket += Socket_NewPacket;

            this.socket.Open(localIp, localSubnetMask);
        }

        protected abstract void Socket_NewPacket(object sender, Sockets.NewPacketEventArgs<ArtNet.Packets.ArtNetPacket> e);

        public void Dispose()
        {
            this.socket.Close();
            this.socket.Dispose();
        }
    }
}
