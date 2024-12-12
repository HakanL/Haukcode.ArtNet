using Haukcode.ArtNet;
using Haukcode.ArtNet.Packets;
using Haukcode.Sockets;
using System;
using System.Net;

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
