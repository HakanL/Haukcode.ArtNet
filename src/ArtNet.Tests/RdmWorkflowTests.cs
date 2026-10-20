using System.Net;
using System.Net.Sockets;
using System.Text;

using Haukcode.ArtNet.Packets;
using Haukcode.ArtNet.Rdm;
using Haukcode.ArtNet.Rdm.IO;
using Haukcode.ArtNet.Rdm.Packets.Discovery;
using Haukcode.ArtNet.Rdm.Packets.DMX;
using Haukcode.ArtNet.Rdm.Packets.Management;
using Haukcode.ArtNet.Rdm.Packets.Product;
using Haukcode.ArtNet.Rdm.Packets.Status;

namespace Haukcode.ArtNet.Tests;

/// <summary>
/// ACK_OVERFLOW writing, UTF-8 text, request/response matching with the ACK_OVERFLOW and
/// ACK_TIMER follow-ups, and direct-line discovery (#34).
/// </summary>
[TestClass]
public class RdmWorkflowTests
{
    private static readonly UId Device = new(0x4744, 0x00000123);

    [TestMethod]
    public void WriteResponseMessages_SplitsOnRecordBoundaries_AsInTheSpecExample()
    {
        // E1.20 6.3.2: 43 proxied UIDs go out as 38 (PDL 0xE4) then 5 (PDL 0x1E)
        var reply = new ProxiedDevices.GetReply { DeviceIds = Enumerable.Range(1, 43).Select(i => new UId(0x1234, (uint)i)).ToList() };

        var messages = RdmPacket.WriteResponseMessages(reply, recordSize: 6);

        Assert.HasCount(2, messages);
        Assert.AreEqual((byte)0xE4, messages[0][21]);
        Assert.AreEqual((byte)0x1E, messages[1][21]);
        Assert.AreEqual((byte)RdmResponseTypes.AckOverflow, messages[0][14]);
        Assert.AreEqual((byte)RdmResponseTypes.Ack, messages[1][14]);
        Assert.IsTrue(messages.All(m => RdmPacket.IsChecksumValid(m)));

        var joined = Assert.IsInstanceOfType<ProxiedDevices.GetReply>(RdmPacket.ReadOverflowPacket(messages.Select(m => (ReadOnlyMemory<byte>)m)));
        CollectionAssert.AreEqual(reply.DeviceIds, joined.DeviceIds);
    }

    [TestMethod]
    public void WriteResponseMessages_KeepsLeadingFieldsInTheFirstMessageOnly()
    {
        // 3 leading bytes + 100 two-byte records: the first message fits 3 + 114 * 2, the second the rest
        var data = new byte[3 + 200];
        var messages = RdmPacket.WriteResponseMessages(new RawReply(data), recordSize: 2, leadingLength: 3);

        Assert.HasCount(1, messages, "203 bytes fit in one message");

        data = new byte[3 + 300];
        messages = RdmPacket.WriteResponseMessages(new RawReply(data), recordSize: 2, leadingLength: 3);

        Assert.HasCount(2, messages);
        Assert.AreEqual((byte)(3 + 114 * 2), messages[0][21]);
        Assert.AreEqual((byte)(300 - 114 * 2), messages[1][21]);
    }

    [TestMethod]
    public void WriteString_UsesUtf8OnlyWhenAllowed_AndTruncatesWholeCharacters()
    {
        Assert.AreEqual("Lyse?", Encoding.ASCII.GetString(WriteText("Lyseø", 32, unicode: false)));
        Assert.AreEqual("Lyseø", Encoding.UTF8.GetString(WriteText("Lyseø", 32, unicode: true)));

        // E1.20-2025 10.1.1: "รู้" is one character of 9 bytes; cutting it would change its meaning
        Assert.AreEqual("abc", Encoding.UTF8.GetString(WriteText("abcรู้", 8, unicode: true)));
        Assert.AreEqual("abcรู้", Encoding.UTF8.GetString(WriteText("abcรู้", 12, unicode: true)));
    }

    private static byte[] WriteText(string text, int maxLength, bool unicode)
    {
        using var stream = new MemoryStream();
        new RdmBinaryWriter(stream) { AllowUnicode = unicode }.WriteString(text, maxLength);

        return stream.ToArray();
    }

    [TestMethod]
    public async Task RequestAsync_ReturnsTheMatchingResponse()
    {
        using var node = new FakeNode();
        using var client = node.CreateClient();

        var request = client.RequestAsync(new DeviceLabel.Get(), node.EndPoint, 0x31, Device);

        var received = await node.ReceiveAsync();

        // A response for another transaction must not complete the request
        await node.ReplyAsync(received, new DeviceLabel.GetReply { Label = "Wrong" }, transactionNumber: (byte)(received.Header.TransactionNumber + 1));
        await node.ReplyAsync(received, new DeviceLabel.GetReply { Label = "Fixture" });

        var reply = Assert.IsInstanceOfType<DeviceLabel.GetReply>(await request);
        Assert.AreEqual("Fixture", reply.Label);
    }

    [TestMethod]
    public async Task RequestAsync_ReturnsNack()
    {
        using var node = new FakeNode();
        using var client = node.CreateClient();

        var request = client.RequestAsync(new DeviceLabel.Get(), node.EndPoint, 0x31, Device);
        var received = await node.ReceiveAsync();
        await node.ReplyAsync(received, new RdmNack(RdmCommands.GetResponse, RdmParameters.DeviceLabel) { Reason = NackReason.DataOutOfRange });

        Assert.AreEqual(NackReason.DataOutOfRange, Assert.IsInstanceOfType<RdmNack>(await request).Reason);
    }

    [TestMethod]
    public async Task RequestAsync_RepeatsTheRequestForAckOverflow()
    {
        using var node = new FakeNode();
        using var client = node.CreateClient();

        var devices = Enumerable.Range(1, 43).Select(i => new UId(0x1234, (uint)i)).ToList();
        var request = client.RequestAsync(new ProxiedDevices.Get(), node.EndPoint, 0x31, Device);

        for (int part = 0; part < 2; part++)
        {
            var received = await node.ReceiveAsync();
            Assert.AreEqual(RdmParameters.ProxiedDevices, received.Header.ParameterId);
            await node.ReplyAsync(received, new ProxiedDevices.GetReply { DeviceIds = devices }, recordSize: 6, part: part);
        }

        CollectionAssert.AreEqual(devices, Assert.IsInstanceOfType<ProxiedDevices.GetReply>(await request).DeviceIds);
    }

    [TestMethod]
    public async Task RequestAsync_FetchesTheDeferredResponseWithQueuedMessage()
    {
        using var node = new FakeNode();
        using var client = node.CreateClient();

        var request = client.RequestAsync(new DeviceLabel.Get(), node.EndPoint, 0x31, Device);

        var received = await node.ReceiveAsync();
        await node.ReplyAsync(received, new RdmAckTimer(RdmCommands.GetResponse, RdmParameters.DeviceLabel) { EstimatedResponseTime = 1 });

        // Nothing queued yet, then another queued message, then the deferred response
        received = await node.ReceiveAsync();
        Assert.AreEqual(RdmParameters.QueuedMessage, received.Header.ParameterId);
        await node.ReplyAsync(received, new StatusMessage.GetReply { StatusMessages = [] });

        received = await node.ReceiveAsync();
        Assert.AreEqual(RdmParameters.QueuedMessage, received.Header.ParameterId);
        await node.ReplyAsync(received, new DmxStartAddress.GetReply { DmxAddress = 17 });

        received = await node.ReceiveAsync();
        Assert.AreEqual(RdmParameters.QueuedMessage, received.Header.ParameterId);
        await node.ReplyAsync(received, new DeviceLabel.GetReply { Label = "Later" });

        Assert.AreEqual("Later", Assert.IsInstanceOfType<DeviceLabel.GetReply>(await request).Label);
    }

    [TestMethod]
    public async Task RequestAsync_TimesOut()
    {
        using var node = new FakeNode();
        using var client = node.CreateClient();

        await Assert.ThrowsExactlyAsync<TimeoutException>(() =>
            client.RequestAsync(new DeviceLabel.Get(), node.EndPoint, 0x31, Device, TimeSpan.FromMilliseconds(200)));
    }

    [TestMethod]
    public async Task Discovery_FindsEveryDevice_IncludingNeighbours()
    {
        var random = new Random(34);
        var devices = Enumerable.Range(0, 40)
            .Select(_ => new UId((ushort)random.Next(1, 0x7FFF), (uint)random.Next()))
            .Concat([new UId(0x4744, 1), new UId(0x4744, 2), new UId(0x4744, 3), new UId(0, 0), new UId(0xFFFF, 0xFFFFFFFE)])
            .Distinct()
            .ToList();
        var line = new SimulatedLine(devices);

        var found = await new RdmDiscovery(line.UniqueBranch, line.Mute, line.UnMuteAll).DiscoverAsync();

        CollectionAssert.AreEquivalent(devices, found.ToList());

        // Nothing new on a second, incremental pass: everything is still muted
        Assert.IsEmpty(await new RdmDiscovery(line.UniqueBranch, line.Mute, line.UnMuteAll).DiscoverMoreAsync());
    }

    /// <summary>Responders on one DMX line: several answering DISC_UNIQUE_BRANCH at once collide.</summary>
    private sealed class SimulatedLine(List<UId> devices)
    {
        private readonly HashSet<UId> muted = [];

        private static ulong Value(UId id) => ((ulong)id.ManufacturerId << 32) | id.DeviceId;

        public Task<byte[]?> UniqueBranch(UId lower, UId upper, CancellationToken cancellationToken)
        {
            var answering = devices.Where(d => !this.muted.Contains(d) && Value(d) >= Value(lower) && Value(d) <= Value(upper)).ToList();
            if (answering.Count == 0)
                return Task.FromResult<byte[]?>(null);

            // The line ORs-and-ANDs colliding signals; ORing the encodings is a fair stand-in
            var response = DiscoveryUniqueBranchResponse.Encode(answering[0]);
            foreach (var device in answering.Skip(1))
            {
                var other = DiscoveryUniqueBranchResponse.Encode(device);
                for (int i = 0; i < response.Length; i++)
                    response[i] |= other[i];
            }

            return Task.FromResult<byte[]?>(response);
        }

        public Task<bool> Mute(UId device, CancellationToken cancellationToken)
        {
            if (!devices.Contains(device))
                return Task.FromResult(false);

            this.muted.Add(device);
            return Task.FromResult(true);
        }

        public Task UnMuteAll(CancellationToken cancellationToken)
        {
            this.muted.Clear();
            return Task.CompletedTask;
        }
    }

    /// <summary>A response with arbitrary parameter data.</summary>
    private sealed class RawReply(byte[] data) : RdmResponsePacket(RdmCommands.GetResponse, RdmParameters.SupportedParameters)
    {
        protected override void ReadData(RdmBinaryReader reader)
        {
        }

        protected override void WriteData(RdmBinaryWriter writer) => writer.WriteBytes(data);
    }

    /// <summary>An Art-Net node on loopback that answers ArtRdm requests by hand.</summary>
    private sealed class FakeNode : IDisposable
    {
        private readonly UdpClient socket = new(new IPEndPoint(IPAddress.Loopback, 0));
        private readonly int clientPort = GetFreeUdpPort();

        public IPEndPoint EndPoint => (IPEndPoint)this.socket.Client.LocalEndPoint!;

        public ArtNetRdmClient CreateClient() =>
            new(IPAddress.Loopback, IPAddress.Parse("255.0.0.0"), port: this.clientPort, rdmId: new UId(0x7FF0, 0x00000001));

        public async Task<RdmPacket> ReceiveAsync()
        {
            using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var result = await this.socket.ReceiveAsync(timeout.Token);
            var artRdm = Assert.IsInstanceOfType<ArtRdmPacket>(ArtNetPacket.Parse(result.Buffer));

            return RdmPacket.ReadPacket(artRdm.RdmData);
        }

        public async Task ReplyAsync(RdmPacket request, RdmResponsePacket reply, byte? transactionNumber = null, int recordSize = 1, int part = 0)
        {
            reply.Header.DestinationId = request.Header.SourceId;
            reply.Header.SourceId = request.Header.DestinationId;
            reply.Header.TransactionNumber = transactionNumber ?? request.Header.TransactionNumber;
            reply.Header.SubDevice = request.Header.SubDevice;

            var artRdm = new ArtRdmPacket
            {
                SubStartCode = (byte)RdmVersions.SubMessage,
                Address = 0x31,
                RdmData = RdmPacket.WriteResponseMessages(reply, recordSize)[part]
            };

            var buffer = new byte[ArtNetPacket.MAX_PACKET_SIZE];
            int length = artRdm.WriteToBuffer(buffer);
            await this.socket.SendAsync(buffer.AsMemory(0, length), new IPEndPoint(IPAddress.Loopback, this.clientPort));
        }

        public void Dispose() => this.socket.Dispose();

        private static int GetFreeUdpPort()
        {
            using var probe = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            probe.Bind(new IPEndPoint(IPAddress.Loopback, 0));

            return ((IPEndPoint)probe.LocalEndPoint!).Port;
        }
    }
}
