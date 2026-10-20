using System.Net;
using System.Net.Sockets;
using System.Threading.Channels;

using Haukcode.ArtNet.Packets;
using Haukcode.ArtNet.Rdm;
using Haukcode.ArtNet.Rdm.IO;
using Haukcode.ArtNet.Rdm.Packets.Control;
using Haukcode.ArtNet.Rdm.Packets.Discovery;
using Haukcode.ArtNet.Rdm.Packets.Product;

namespace Haukcode.ArtNet.Tests;

[TestClass]
public class WireComplianceTests
{
    // MessageLength (1) + DestinationId (6) + SourceId (6) + TransactionNumber (1)
    private const int PortOrResponseTypeOffset = 14;

    private static byte[] Encode(RdmPacket packet)
    {
        using var stream = new MemoryStream();
        RdmPacket.WritePacket(packet, new RdmBinaryWriter(stream));
        return stream.ToArray();
    }

    [TestMethod]
    public void RdmRequests_DefaultToPortId1()
    {
        // E1.20 6.2.7.1: Port ID 1-255 for GET, SET and DISCOVERY commands; 0 is NACKed
        Assert.AreEqual((byte)1, Encode(new DeviceLabel.Get())[PortOrResponseTypeOffset]);
        Assert.AreEqual((byte)1, Encode(new CapturePreset.Set { SceneNumber = 1 })[PortOrResponseTypeOffset]);
        Assert.AreEqual((byte)1, Encode(new DiscoveryMute.Request())[PortOrResponseTypeOffset]);
    }

    [TestMethod]
    public void CapturePresetSet_ParsesTheSceneOnlyForm()
    {
        var header = new RdmHeader
        {
            Command = RdmCommands.Set,
            ParameterId = RdmParameters.CapturePreset,
            ParameterDataLength = 2,
        };

        using var content = new MemoryStream([0x00, 0x07]);
        var set = Assert.IsInstanceOfType<CapturePreset.Set>(RdmPacket.ReadPacket(header, new RdmBinaryReader(content)));

        Assert.AreEqual((ushort)7, set.SceneNumber);
        Assert.AreEqual((ushort)0, set.UpFadeTime);
    }

    [TestMethod]
    public async Task ArtTodData_With200Uids_IsReceived()
    {
        int port = GetFreeUdpPort();
        var received = Channel.CreateUnbounded<int>();

        using var client = new ArtNetClient(
            IPAddress.Loopback,
            IPAddress.Parse("255.0.0.0"),
            channelWriter: p =>
            {
                if (p.Packet is ArtTodDataPacket tod)
                    received.Writer.TryWrite(tod.Devices.Count);

                return Task.CompletedTask;
            },
            port: port);

        var packet = new ArtTodDataPacket { Port = 1, UIdTotal = 200, Devices = [.. Enumerable.Range(1, 200).Select(i => new UId(0x1234, (uint)i))] };
        var buffer = new byte[ArtNetPacket.MAX_PACKET_SIZE];
        int length = packet.WriteToBuffer(buffer);
        Assert.AreEqual(1228, length);

        using (var sender = new UdpClient(new IPEndPoint(IPAddress.Loopback, 0)))
            await sender.SendAsync(buffer.AsMemory(0, length), new IPEndPoint(IPAddress.Loopback, port));

        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        Assert.AreEqual(200, await received.Reader.ReadAsync(timeout.Token));
    }

    private static int GetFreeUdpPort()
    {
        using var probe = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        probe.Bind(new IPEndPoint(IPAddress.Loopback, 0));

        return ((IPEndPoint)probe.LocalEndPoint!).Port;
    }
}
