using System;
using System.Net;

namespace Haukcode.Sockets
{
    public class NewPacketEventArgs<TPacketType> : EventArgs
    {
        public NewPacketEventArgs(IPEndPoint source, IPEndPoint destination, TPacketType packet)
        {
            Source = source;
            Destination = destination;
            Packet = packet;
        }

        private IPEndPoint source;

        public IPEndPoint Source
        {
            get { return source; }
            protected set { source = value; }
        }

        private IPEndPoint destination;

        public IPEndPoint Destination
        {
            get { return destination; }
            protected set { destination = value; }
        }

        private TPacketType packet;

        public TPacketType Packet
        {
            get { return packet; }
            private set { packet = value; }
        }
    }
}
