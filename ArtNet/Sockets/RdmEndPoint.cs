using Haukcode.Rdm;
using System.Collections.Generic;
using System.Net;

namespace Haukcode.Sockets
{
    public class RdmEndPoint : IPEndPoint
    {
        public RdmEndPoint(IPAddress ipAddress)
            : this(ipAddress, 0, 0)
        {
        }

        public RdmEndPoint(IPAddress ipAddress, int universe)
            : this(ipAddress, 0, universe)
        {
        }

        public RdmEndPoint(IPEndPoint ipEndPoint)
            : this(ipEndPoint.Address, ipEndPoint.Port, 0)
        {
        }

        public RdmEndPoint(IPEndPoint ipEndPoint, int universe)
            : this(ipEndPoint.Address, ipEndPoint.Port, universe)
        {
        }

        public RdmEndPoint(IPAddress ipAddress, int port, int universe) : base(ipAddress, port)
        {
            IpAddress = ipAddress;
            Universe = universe;
        }

        private IPAddress ipAddress = IPAddress.Any;

        public IPAddress IpAddress
        {
            get { return ipAddress; }
            set { ipAddress = value; }
        }

        private UId id = UId.Empty;

        public UId Id
        {
            get { return id; }
            set { id = value; }
        }

        private UId gatewayId = UId.Empty;

        public UId GatewayId
        {
            get { return gatewayId; }
            set { gatewayId = value; }
        }

        private int universe = 0;

        public int Universe
        {
            get { return universe; }
            set { universe = value; }
        }

        public override string ToString()
        {
            return IpAddress.ToString();
        }
    }

    public class RdmEndpointComparer : IEqualityComparer<RdmEndPoint>
    {

        public bool Equals(RdmEndPoint x, RdmEndPoint y)
        {
            return x.Id.Equals(y.Id) && x.Universe.Equals(y.Universe);
        }

        public int GetHashCode(RdmEndPoint obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
