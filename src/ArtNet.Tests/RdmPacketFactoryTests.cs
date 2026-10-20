using Haukcode.ArtNet.Rdm;
using Haukcode.ArtNet.Rdm.IO;
using Haukcode.ArtNet.Rdm.Packets;
using Haukcode.ArtNet.Rdm.Packets.Discovery;
using Haukcode.ArtNet.Rdm.Packets.Management;

namespace Haukcode.ArtNet.Tests;

[TestClass]
public class RdmPacketFactoryTests
{
    // Packets that are built outside the (command, PID) lookup on purpose.
    private static readonly HashSet<Type> NotRegistered =
    [
        typeof(RdmRawPacket),
        typeof(RdmNack),
        typeof(RdmAckTimer),
    ];

    private static RdmPacket RoundTrip(RdmPacket packet)
    {
        using var stream = new MemoryStream();
        RdmPacket.WritePacket(packet, new RdmBinaryWriter(stream));

        stream.Position = 0;
        return RdmPacket.ReadPacket(new RdmBinaryReader(stream));
    }

    [TestMethod]
    public void CommsStatusGetReply_RoundTrip_ReturnsTypedPacket()
    {
        var packet = new CommsStatus.GetReply
        {
            ShortMessage = 1,
            LengthMismatch = 2,
            ChecksumFail = 3,
        };

        var result = RoundTrip(packet);

        var reply = Assert.IsInstanceOfType<CommsStatus.GetReply>(result);
        Assert.AreEqual(1, reply.ShortMessage);
        Assert.AreEqual(2, reply.LengthMismatch);
        Assert.AreEqual(3, reply.ChecksumFail);
    }

    [TestMethod]
    public void DiscoveryUnMuteReply_RoundTrip_ReturnsUnMuteType()
    {
        var packet = new DiscoveryUnMute.Reply
        {
            ControlField = 1,
            BindingId = new UId(0x1234, 0x56789ABC),
        };

        var result = RoundTrip(packet);

        var reply = Assert.IsInstanceOfType<DiscoveryUnMute.Reply>(result);
        Assert.AreEqual(RdmParameters.DiscoveryUnMute, reply.Header.ParameterId);
        Assert.AreEqual(1, reply.ControlField);
        Assert.AreEqual(packet.BindingId, reply.BindingId);
    }

    [TestMethod]
    public void DiscoveryUnMuteRequest_RoundTrip_ReturnsUnMuteType()
    {
        var result = RoundTrip(new DiscoveryUnMute.Request());

        Assert.IsInstanceOfType<DiscoveryUnMute.Request>(result);
    }

    [TestMethod]
    public void EveryPacketType_IsRegisteredForItsOwnCommandAndParameter()
    {
        var packetTypes = typeof(RdmPacket).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(RdmPacket).IsAssignableFrom(t) && !NotRegistered.Contains(t))
            .ToList();

        Assert.IsNotEmpty(packetTypes);

        var failures = new List<string>();
        foreach (var type in packetTypes)
        {
            if (type.GetConstructor(Type.EmptyTypes) == null)
            {
                failures.Add($"{type.FullName}: no parameterless constructor");
                continue;
            }

            var packet = (RdmPacket)Activator.CreateInstance(type)!;
            var built = RdmPacketFactory.Build(packet.Header);

            if (built?.GetType() != type)
            {
                failures.Add($"{type.FullName}: {packet.Header.Command}/{packet.Header.ParameterId} builds {built?.GetType().FullName ?? "null"}");
            }
        }

        Assert.IsEmpty(failures, string.Join(Environment.NewLine, failures));
    }

    [TestMethod]
    public void EveryPacketType_DerivesFromBaseMatchingItsCommand()
    {
        var failures = new List<string>();
        foreach (var type in typeof(RdmPacket).Assembly.GetTypes())
        {
            if (!type.IsClass || type.IsAbstract || !typeof(RdmPacket).IsAssignableFrom(type) || NotRegistered.Contains(type))
                continue;

            var command = ((RdmPacket)Activator.CreateInstance(type)!).Header.Command;
            bool isResponse = command is RdmCommands.GetResponse or RdmCommands.SetResponse or RdmCommands.DiscoveryResponse;
            var expectedBase = isResponse ? typeof(RdmResponsePacket) : typeof(RdmRequestPacket);

            if (!expectedBase.IsAssignableFrom(type))
                failures.Add($"{type.FullName}: {command} should derive from {expectedBase.Name}");
        }

        Assert.IsEmpty(failures, string.Join(Environment.NewLine, failures));
    }
}
