using System.Net;
using System.Net.Sockets;
using System.Threading.Channels;

using Haukcode.ArtNet.Packets;

namespace Haukcode.ArtNet.Tests;

[TestClass]
public class ArtNetClientTests
{
    [TestMethod]
    public void NextSequence_SkipsZero()
    {
        Assert.AreEqual((byte)1, ArtNetClient.NextSequence(0));
        Assert.AreEqual((byte)2, ArtNetClient.NextSequence(1));
        Assert.AreEqual((byte)255, ArtNetClient.NextSequence(254));
        Assert.AreEqual((byte)1, ArtNetClient.NextSequence(255));
    }

    [TestMethod]
    public void SendDmxData_RejectsUniverseOutsidePortAddressRange()
    {
        using var client = new ArtNetClient(IPAddress.Loopback, IPAddress.Parse("255.0.0.0"), port: GetFreeUdpPort());

        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => client.SendDmxData(IPAddress.Loopback, 0, new byte[2]));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => client.SendDmxData(IPAddress.Loopback, ArtNetClient.MaxUniverseId + 1, new byte[2]));
    }

    [TestMethod]
    public async Task Receive_DropsOwnPacketsButKeepsOthers()
    {
        int port = GetFreeUdpPort();
        var received = Channel.CreateUnbounded<ushort>();

        using var client = new ArtNetClient(
            IPAddress.Loopback,
            IPAddress.Parse("255.0.0.0"),
            channelWriter: p =>
            {
                // ArtDmx arrives in a reused instance, so copy what we need before returning
                if (p.Packet is ArtNetDmxPacket dmx)
                    received.Writer.TryWrite(dmx.Universe);

                return Task.CompletedTask;
            },
            port: port);

        // Our own DMX, sent from the client's ephemeral send socket to its own listen port
        for (int i = 0; i < 5; i++)
            await client.SendDmxData(IPAddress.Loopback, 1, new byte[2]);

        // Then the same packet type from a different socket
        var other = new ArtNetDmxPacket { Universe = 41, Sequence = 1, DmxData = new byte[2] };
        var buffer = new byte[other.PacketLength];
        int length = other.WriteToBuffer(buffer);
        using (var sender = new UdpClient(new IPEndPoint(IPAddress.Loopback, 0)))
        {
            await Task.Delay(200);
            await sender.SendAsync(buffer.AsMemory(0, length), new IPEndPoint(IPAddress.Loopback, port));
        }

        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        ushort first = await received.Reader.ReadAsync(timeout.Token);

        Assert.AreEqual((ushort)41, first, "The client's own packets should have been dropped");
        Assert.IsFalse(received.Reader.TryRead(out _));
    }

    private static int GetFreeUdpPort()
    {
        using var probe = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        probe.Bind(new IPEndPoint(IPAddress.Loopback, 0));

        return ((IPEndPoint)probe.LocalEndPoint!).Port;
    }
}
