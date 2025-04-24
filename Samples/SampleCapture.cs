using System;
using System.Net;
using Haukcode.ArtNet.Internal;

namespace Haukcode.ArtNet.Samples;

public class SampleCapture : SampleBase
{
    public SampleCapture(IPAddress localIp, IPAddress localSubnetMask)
        : base(localIp, localSubnetMask)
    {
    }

    protected override void Socket_NewPacket(double timestampMS, ReceiveDataPacket e)
    {
        Console.WriteLine($"Received ArtNet packet with OpCode: {e.Packet.OpCode} from {e.Source}");
    }
}
