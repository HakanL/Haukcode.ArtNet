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

    [TestMethod]
    public async Task Broadcast_IsRefusedForPacketsArtNet4Forbids()
    {
        using var client = new ArtNetClient(IPAddress.Loopback, IPAddress.Parse("255.0.0.0"), port: GetFreeUdpPort());

        // ArtDmx "must be unicast to subscribers"
        Assert.ThrowsExactly<InvalidOperationException>(() => client.SendDmxData(null, 1, new byte[2]));
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => client.QueuePacketForSending((IPAddress?)null, new ArtRdmPacket { RdmData = [] }));
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => client.QueuePacketForSending((IPAddress?)null, new ArtTodDataPacket { Devices = [] }));

        // Opt-in for ArtDmx, and ArtPoll may always be broadcast
        client.AllowDmxBroadcast = true;
        _ = client.SendDmxData(null, 1, new byte[2]);
        _ = client.QueuePacketForSending((IPAddress?)null, new ArtPollPacket());
    }

    [TestMethod]
    public async Task Packets_LeaveFromTheArtNetPort()
    {
        int port = GetFreeUdpPort();
        using var receiver = new UdpClient(new IPEndPoint(IPAddress.Loopback, 0));

        using var client = new ArtNetClient(IPAddress.Loopback, IPAddress.Parse("255.0.0.0"), port: port);
        await client.SendPacketImmediately((IPEndPoint)receiver.Client.LocalEndPoint!, new ArtPollPacket());

        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var result = await receiver.ReceiveAsync(timeout.Token);

        // "The UDP port used as both source and destination is 0x1936" (here: the client's port)
        Assert.AreEqual(port, result.RemoteEndPoint.Port);
    }

    [TestMethod]
    public async Task Receive_IgnoresProtocolVersionsBelow14()
    {
        int port = GetFreeUdpPort();
        var received = Channel.CreateUnbounded<ushort>();

        using var client = new ArtNetClient(
            IPAddress.Loopback,
            IPAddress.Parse("255.0.0.0"),
            channelWriter: p =>
            {
                if (p.Packet is ArtNetDmxPacket dmx)
                    received.Writer.TryWrite(dmx.Universe);

                return Task.CompletedTask;
            },
            port: port);

        var buffer = new byte[ArtNetPacket.MAX_PACKET_SIZE];
        int length = new ArtNetDmxPacket { Universe = 1, Sequence = 1, DmxData = new byte[2] }.WriteToBuffer(buffer);

        using var sender = new UdpClient(new IPEndPoint(IPAddress.Loopback, 0));

        var old = buffer[..length];
        old[11] = 13; // ProtVer Lo
        await sender.SendAsync(old, new IPEndPoint(IPAddress.Loopback, port));

        // Same ArtDmx with version 14, and universe 2 (SubUni at offset 14) to tell them apart
        var current = buffer[..length];
        current[14] = 2;
        await sender.SendAsync(current, new IPEndPoint(IPAddress.Loopback, port));

        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        Assert.AreEqual((ushort)2, await received.Reader.ReadAsync(timeout.Token), "the version 13 packet should have been ignored");
    }

    private static int GetFreeUdpPort()
    {
        using var probe = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        probe.Bind(new IPEndPoint(IPAddress.Loopback, 0));

        return ((IPEndPoint)probe.LocalEndPoint!).Port;
    }
}
