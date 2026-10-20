using System.Net;
using System.Net.Sockets;

using Haukcode.ArtNet.Packets;
using Haukcode.ArtNet.Rdm;
using Haukcode.ArtNet.Rdm.Packets.Product;

namespace Haukcode.ArtNet.Tests;

[TestClass]
public class ArtNetRdmClientTests
{
    // MessageLength (1) + DestinationId (6) + SourceId (6)
    private const int TransactionNumberOffset = 13;

    [TestMethod]
    public async Task SendRdm_AssignsIncreasingTransactionNumbersAndThePortAddress()
    {
        using var receiver = new UdpClient(new IPEndPoint(IPAddress.Loopback, 0));
        var target = (IPEndPoint)receiver.Client.LocalEndPoint!;

        using var client = new ArtNetRdmClient(IPAddress.Loopback, IPAddress.Parse("255.0.0.0"), port: GetFreeUdpPort());
        var device = new UId(0x1234, 0x56789ABC);

        await client.SendRdm(new DeviceLabel.Get(), target, 0x0731, device);
        await client.SendRdm(new DeviceLabel.Get(), target, 0x0731, device);

        var first = await ReceiveArtRdm(receiver);
        var second = await ReceiveArtRdm(receiver);

        Assert.AreEqual((byte)1, first.RdmData[TransactionNumberOffset]);
        Assert.AreEqual((byte)2, second.RdmData[TransactionNumberOffset]);

        // Port-Address 0x0731 = Net 7, Sub-Net/Universe 0x31
        Assert.AreEqual((byte)0x07, first.Net);
        Assert.AreEqual((byte)0x31, first.Address);
    }

    private static async Task<ArtRdmPacket> ReceiveArtRdm(UdpClient receiver)
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var result = await receiver.ReceiveAsync(timeout.Token);

        return Assert.IsInstanceOfType<ArtRdmPacket>(ArtNetPacket.Parse(result.Buffer));
    }

    private static int GetFreeUdpPort()
    {
        using var probe = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        probe.Bind(new IPEndPoint(IPAddress.Loopback, 0));

        return ((IPEndPoint)probe.LocalEndPoint!).Port;
    }
}
