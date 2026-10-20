using Haukcode.ArtNet.Packets;
using Haukcode.ArtNet.Rdm;
using Haukcode.ArtNet.Rdm.IO;
using Haukcode.ArtNet.Rdm.Packets.DMX;
using Haukcode.ArtNet.Rdm.Packets.Management;
using Haukcode.ArtNet.Rdm.Packets.Product;
using Haukcode.ArtNet.Rdm.Packets.Status;

namespace Haukcode.ArtNet.Tests;

/// <summary>
/// Enum values added from Art-Net 4 rev 1.4dp (#28) and E1.20-2025 (#29).
/// </summary>
[TestClass]
public class SpecEnumTests
{
    [TestMethod]
    public void UniverseAddress_UsesOnlyTheLowNibbleOfSwOut()
    {
        var reply = new ArtPollReplyPacket { NetSwitch = 0, SubSwitch = 0, SwOut = [0xF3, 0, 0, 0] };

        Assert.AreEqual(3, reply.UniverseAddress(outPorts: true, 0));
    }

    /// <summary>
    /// Looks the member up by name at runtime, so a renamed or renumbered member fails here.
    /// </summary>
    [TestMethod]
    // Art-Net 4 rev 1.4dp
    [DataRow(typeof(ArtNetStyles), nameof(ArtNetStyles.StController), 0x01)]
    [DataRow(typeof(ArtNetStyles), nameof(ArtNetStyles.StVisual), 0x06)]
    [DataRow(typeof(ArtAddressCommand), nameof(ArtAddressCommand.AcBqp0), 0xE0)]
    [DataRow(typeof(ArtAddressCommand), nameof(ArtAddressCommand.AcBqp15), 0xEF)]
    [DataRow(typeof(ArtTodControlCommand), nameof(ArtTodControlCommand.AtcEnd), 0x02)]
    [DataRow(typeof(ArtTodControlCommand), nameof(ArtTodControlCommand.AtcIncOff), 0x04)]
    [DataRow(typeof(PollReplyPortTypes), nameof(PollReplyPortTypes.Dali), 0x06)]
    // E1.20-2025
    [DataRow(typeof(StatusTypes), nameof(StatusTypes.ErrorCleared), 0x14)]
    [DataRow(typeof(RdmResponseTypes), nameof(RdmResponseTypes.AckTimerHiRes), 0x04)]
    [DataRow(typeof(ProductCategories), nameof(ProductCategories.DimmerAcOther), 0x0506)]
    [DataRow(typeof(ProductDetailIdList.DetailId), nameof(ProductDetailIdList.DetailId.EthernetNode), 0x0601)]
    [DataRow(typeof(ProductDetailIdList.DetailId), nameof(ProductDetailIdList.DetailId.Other), 0x7FFF)]
    [DataRow(typeof(SlotIds), nameof(SlotIds.ColorAddLime), 0x020A)]
    [DataRow(typeof(SlotIds), nameof(SlotIds.ColorSemaphore), 0x0210)]
    [DataRow(typeof(SlotIds), nameof(SlotIds.ColorAddUV), 0x0218)]
    [DataRow(typeof(SlotIds), nameof(SlotIds.FountainControl), 0x0508)]
    [DataRow(typeof(NackReason), nameof(NackReason.ActionNotSupported), 0x0B)]
    [DataRow(typeof(NackReason), nameof(NackReason.UnknownUid), 0x0E)]
    [DataRow(typeof(NackReason), nameof(NackReason.InvalidPort), 0x13)]
    [DataRow(typeof(NackReason), nameof(NackReason.AllCallSetFail), 0x20)]
    [DataRow(typeof(Haukcode.ArtNet.Rdm.Packets.Sensors.SensorDefinition.SensorTypes), "CpuLoad", 0x21)]
    [DataRow(typeof(Haukcode.ArtNet.Rdm.Packets.Sensors.SensorDefinition.SensorTypes), "LogRatioWatts", 0x28)]
    [DataRow(typeof(Haukcode.ArtNet.Rdm.Packets.Sensors.SensorDefinition.SensorUnit), "Decibel", 0x1D)]
    [DataRow(typeof(Haukcode.ArtNet.Rdm.Packets.Sensors.SensorDefinition.SensorUnit), "BytesPerSecond", 0x24)]
    [DataRow(typeof(RdmParameters), "QueuedMessageSensorSubscribe", 0x0034)]
    [DataRow(typeof(RdmParameters), "NackDescription", 0x0057)]
    [DataRow(typeof(RdmParameters), "EnumLabel", 0x005A)]
    [DataRow(typeof(RdmParameters), "SelfTestEnhanced", 0x1022)]
    [DataRow(typeof(Haukcode.ArtNet.Rdm.Packets.Parameters.ParameterDataTypes), "Enumeration", 0x12)]
    // E1.37-7
    [DataRow(typeof(Haukcode.ArtNet.Rdm.Packets.Net.DiscoveryState.DiscoveryStates), "Incremental", 0x01)]
    [DataRow(typeof(Haukcode.ArtNet.Rdm.Packets.Net.DiscoveryState.DiscoveryStates), "Full", 0x02)]
    public void EnumValue_MatchesTheSpec(Type enumType, string name, int expected)
    {
        Assert.AreEqual(expected, Convert.ToInt32(Enum.Parse(enumType, name)));
    }

    [TestMethod]
    public void ControllerFlags_ShareTheMessageCountByte()
    {
        var header = new RdmHeader { ControllerFlags = RdmControllerFlags.Unicode | RdmControllerFlags.HiResAckTimerSupport };

        Assert.AreEqual((byte)0x03, header.MessageCount);
    }

    [TestMethod]
    public void AckTimerHiRes_DecodesAsAckTimerInMilliseconds()
    {
        var header = new RdmHeader
        {
            Command = RdmCommands.GetResponse,
            ParameterId = RdmParameters.DeviceLabel,
            PortOrResponseType = (byte)RdmResponseTypes.AckTimerHiRes,
            ParameterDataLength = 2,
        };

        using var content = new MemoryStream([0x01, 0xF4]);
        var timer = Assert.IsInstanceOfType<RdmAckTimer>(RdmPacket.ReadPacket(header, new RdmBinaryReader(content)));

        Assert.AreEqual(TimeSpan.FromMilliseconds(500), timer.EstimatedResponseDelay);
    }
}
