using System;
using System.Net;
using Haukcode.ArtNet;

namespace Haukcode.Samples
{
    public abstract class SampleBase : IDisposable
    {
        protected readonly ArtNetClient socket;

        public SampleBase(IPAddress localIp, IPAddress localSubnetMask)
        {
            //this.socket = new ArtNetClient();

            //this.socket.NewPacket += Socket_NewPacket;

            //this.socket.Open(localIp, localSubnetMask);
        }

        protected abstract void Socket_NewPacket(object sender, Sockets.NewPacketEventArgs<ArtNet.Packets.ArtNetPacket> e);

        public void Dispose()
        {
            //this.socket.Close();
            this.socket.Dispose();
        }
    }
}
