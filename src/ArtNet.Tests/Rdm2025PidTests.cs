using Haukcode.ArtNet.Rdm;
using Haukcode.ArtNet.Rdm.IO;
using Haukcode.ArtNet.Rdm.Packets.Control;
using Haukcode.ArtNet.Rdm.Packets.Parameters;
using Haukcode.ArtNet.Rdm.Packets.Status;

namespace Haukcode.ArtNet.Tests;

/// <summary>
/// The PIDs added in E1.20-2025 (#32).
/// </summary>
[TestClass]
public class Rdm2025PidTests
{
    private const int ParameterDataLengthOffset = 21;

    private static byte[] Encode(RdmPacket packet)
    {
        using var stream = new MemoryStream();
        RdmPacket.WritePacket(packet, new RdmBinaryWriter(stream));
        return stream.ToArray();
    }

    private static T RoundTrip<T>(T packet, int expectedPdl)
        where T : RdmPacket
    {
        var wire = Encode(packet);
        Assert.AreEqual((byte)expectedPdl, wire[ParameterDataLengthOffset], $"{typeof(T).FullName} PDL");

        using var input = new MemoryStream(wire);
        return Assert.IsInstanceOfType<T>(RdmPacket.ReadPacket(new RdmBinaryReader(input)));
    }

    private static byte[] WithChecksum(byte[] message)
    {
        ushort sum = (ushort)(0xCC + 0x01 + message.Sum(b => b));
        return [.. message, (byte)(sum >> 8), (byte)sum];
    }

    [TestMethod]
    public void QueuedMessageSensorSubscribe_RoundTrips()
    {
        CollectionAssert.AreEqual(new byte[] { 1, 3 }, RoundTrip(new QueuedMessageSensorSubscribe.GetReply { Sensors = [1, 3] }, 2).Sensors);

        var set = RoundTrip(new QueuedMessageSensorSubscribe.Set { Action = QueuedMessageSensorSubscribe.Actions.Add, Sensors = [QueuedMessageSensorSubscribe.AllSensors] }, 2);
        Assert.AreEqual(QueuedMessageSensorSubscribe.Actions.Add, set.Action);
        CollectionAssert.AreEqual(new byte[] { 0xFF }, set.Sensors);
    }

    [TestMethod]
    public void EnumLabel_RoundTrips()
    {
        var get = RoundTrip(new EnumLabel.Get { ParameterId = (RdmParameters)0x8001, EnumerationIndex = 3 }, 6);
        Assert.AreEqual(3u, get.EnumerationIndex);

        var reply = RoundTrip(new EnumLabel.GetReply { ParameterId = (RdmParameters)0x8001, EnumerationIndex = 3, MaximumEnumerationIndex = 7, Label = "Fast" }, 14);
        Assert.AreEqual((RdmParameters)0x8001, reply.ParameterId);
        Assert.AreEqual(7u, reply.MaximumEnumerationIndex);
        Assert.AreEqual("Fast", reply.Label);
    }

    [TestMethod]
    public void SupportedParametersEnhanced_RoundTrips()
    {
        var reply = RoundTrip(new SupportedParametersEnhanced.GetReply
        {
            Parameters =
            [
                new(RdmParameters.DeviceLabel, SupportedParametersEnhanced.PidSupport.Get | SupportedParametersEnhanced.PidSupport.Set),
                new(RdmParameters.SensorValue, SupportedParametersEnhanced.PidSupport.Get | SupportedParametersEnhanced.PidSupport.PackedPidIndexGet),
            ],
        }, 8);

        Assert.AreEqual(2, reply.Parameters.Count);
        Assert.AreEqual(SupportedParametersEnhanced.PidSupport.Get | SupportedParametersEnhanced.PidSupport.PackedPidIndexGet, reply.Parameters[1].Support);
    }

    [TestMethod]
    public void ControllerFlagSupport_And_NackDescription_RoundTrip()
    {
        Assert.AreEqual(RdmControllerFlags.HiResAckTimerSupport, RoundTrip(new ControllerFlagSupport.GetReply { SupportedFlags = RdmControllerFlags.HiResAckTimerSupport }, 1).SupportedFlags);

        Assert.AreEqual((ushort)0x8001, RoundTrip(new NackDescription.Get { ReasonCode = 0x8001 }, 2).ReasonCode);
        Assert.AreEqual("Too hot", RoundTrip(new NackDescription.GetReply { ReasonCode = 0x8001, Description = "Too hot" }, 9).Description);
    }

    [TestMethod]
    public void PackedPidSub_RoundTrips()
    {
        var get = RoundTrip(new PackedPidSub.Get { ParameterId = RdmParameters.DmxStartAddress, FirstDevice = 0 }, 8);
        Assert.AreEqual(PackedPidSub.AllDevices, get.DeviceCount);

        // The spec's example: start addresses of the root device and two sub-devices (PDL 0x13)
        var reply = RoundTrip(new PackedPidSub.GetReply
        {
            ParameterId = RdmParameters.DmxStartAddress,
            Entries = [new(0x0000, [0, 1]), new(0x0001, [0, 2]), new(0x0002, [0, 3])],
        }, 0x13);

        Assert.AreEqual(3, reply.Entries.Count);
        CollectionAssert.AreEqual(new byte[] { 0, 3 }, reply.Entries[2].Data);
        Assert.AreEqual((ushort)2, reply.Entries[2].Id);
    }

    [TestMethod]
    public void PackedPidSub_OverflowContinuationOmitsThePid()
    {
        // First message: PID, index and one entry. Continuation: entries only (E1.20-2025 10.4.7)
        var first = new PackedPidSub.GetReply { ResponseType = RdmResponseTypes.AckOverflow, ParameterId = RdmParameters.DmxStartAddress, Entries = [new(0, [0, 1])] };

        var continuation = Encode(new PackedPidSub.GetReply { Entries = [new(1, [0, 2])] });
        // Drop the PID and index (4 bytes) from the continuation and fix its lengths
        var trimmed = continuation[..22].Concat(continuation[26..]).ToArray();
        trimmed[0] -= 4;
        trimmed[ParameterDataLengthOffset] -= 4;

        var reply = Assert.IsInstanceOfType<PackedPidSub.GetReply>(RdmPacket.ReadOverflowPacket([WithChecksum(Encode(first)), WithChecksum(trimmed)]));

        Assert.AreEqual(RdmParameters.DmxStartAddress, reply.ParameterId);
        Assert.AreEqual(2, reply.Entries.Count);
        Assert.AreEqual((ushort)1, reply.Entries[1].Id);
    }

    [TestMethod]
    public void PackedPidIndex_RoundTrips_AndRejectsABadEntryLength()
    {
        var reply = RoundTrip(new PackedPidIndex.GetReply { ParameterId = RdmParameters.SensorValue, Entries = [new(1, new byte[9]), new(2, new byte[9])] }, 2 + 2 * 12);
        Assert.AreEqual((ushort)2, reply.Entries[1].Id);

        var set = RoundTrip(new PackedPidIndex.Set { ParameterId = RdmParameters.SensorValue, Entries = [new(1, [1])] }, 2 + 4);
        Assert.AreEqual(1, set.Entries.Count);

        // An entry claiming more data than the message has
        var header = new RdmHeader { Command = RdmCommands.GetResponse, ParameterId = RdmParameters.PackedPidIndex, ParameterDataLength = 5 };
        using var content = new MemoryStream([0x02, 0x02, 0x00, 0x01, 0x09]);
        Assert.ThrowsExactly<InvalidPacketException>(() => RdmPacket.ReadPacket(header, new RdmBinaryReader(content)));
    }

    [TestMethod]
    public void SelfTestEnhanced_RoundTrips()
    {
        var reply = RoundTrip(new SelfTestEnhanced.GetReply
        {
            Tests =
            [
                new(1, SelfTestEnhanced.SelfTestStatus.Pass, SelfTestEnhanced.SelfTestCapabilities.AutoTerminates, 0),
                new(2, SelfTestEnhanced.SelfTestStatus.Active, SelfTestEnhanced.SelfTestCapabilities.None, 0),
            ],
        }, 2 + 2 * 6);

        Assert.AreEqual(SelfTestEnhanced.SelfTestStatus.Active, reply.Tests[1].Status);
        Assert.AreEqual(SelfTestEnhanced.SelfTestCapabilities.AutoTerminates, reply.Tests[0].Capabilities);
    }
}
