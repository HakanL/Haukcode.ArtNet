using Haukcode.ArtNet.Rdm;
using Haukcode.ArtNet.Rdm.IO;
using Haukcode.ArtNet.Rdm.Packets.Configuration;
using Haukcode.ArtNet.Rdm.Packets.Control;
using Haukcode.ArtNet.Rdm.Packets.Dimmer;
using Haukcode.ArtNet.Rdm.Packets.Discovery;
using Haukcode.ArtNet.Rdm.Packets.DMX;
using Haukcode.ArtNet.Rdm.Packets.Power;

namespace Haukcode.ArtNet.Tests;

/// <summary>
/// Parameter data layouts per E1.20 / E1.37-1: each packet must write the spec's PDL and read
/// back the same values.
/// </summary>
[TestClass]
public class RdmPacketPayloadTests
{
    // MessageLength, DestinationId (6), SourceId (6), TransactionNumber, PortOrResponseType,
    // MessageCount, SubDevice (2), Command, ParameterId (2)
    private const int ParameterDataLengthOffset = 21;

    private static T RoundTrip<T>(T packet, int expectedPdl)
        where T : RdmPacket
    {
        using var stream = new MemoryStream();
        RdmPacket.WritePacket(packet, new RdmBinaryWriter(stream));
        var wire = stream.ToArray();

        Assert.AreEqual((byte)expectedPdl, wire[ParameterDataLengthOffset], $"{typeof(T).FullName} PDL");

        using var input = new MemoryStream(wire);
        return Assert.IsInstanceOfType<T>(RdmPacket.ReadPacket(new RdmBinaryReader(input)));
    }

    [TestMethod]
    [DataRow((ushort)0x0000, PresetPlayback.PlayMode.Off)]
    [DataRow((ushort)0xFFFF, PresetPlayback.PlayMode.All)]
    [DataRow((ushort)0x0005, PresetPlayback.PlayMode.Scene)]
    public void PresetPlayback_DecodesModeAndKeepsLevel(ushort value, PresetPlayback.PlayMode expectedMode)
    {
        var reply = RoundTrip(new PresetPlayback.GetReply { Mode = expectedMode, SceneNumber = value, Level = 200 }, 3);

        Assert.AreEqual(expectedMode, reply.Mode);
        Assert.AreEqual(value, reply.SceneNumber);
        Assert.AreEqual((byte)200, reply.Level);

        var set = RoundTrip(new PresetPlayback.Set { Mode = expectedMode, SceneNumber = value, Level = 100 }, 3);
        Assert.AreEqual(expectedMode, set.Mode);
        Assert.AreEqual((byte)100, set.Level);
    }

    [TestMethod]
    public void PerformSelfTest_SetCarriesTestNumber_SetReplyIsEmpty()
    {
        Assert.AreEqual((byte)0xFF, RoundTrip(new PerformSelfTest.Set { TestNumber = 0xFF }, 1).TestNumber);
        RoundTrip(new PerformSelfTest.SetReply(), 0);
    }

    [TestMethod]
    public void LampState_SetCarriesState()
    {
        Assert.AreEqual(LampState.LampStates.Strike, RoundTrip(new LampState.Set { LampState = LampState.LampStates.Strike }, 1).LampState);
    }

    [TestMethod]
    public void SlotIds_ColorSemaphore_MatchesSpec()
    {
        Assert.AreEqual(0x0210, (int)SlotIds.ColorSemaphore);
    }

    [TestMethod]
    public void DiscoveryMuteReply_BindingUidIsOptional()
    {
        Assert.IsNull(RoundTrip(new DiscoveryMute.Reply { ControlField = 1 }, 2).BindingId);

        var binding = new UId(0x1234, 0x56789ABC);
        Assert.AreEqual(binding, RoundTrip(new DiscoveryUnMute.Reply { ControlField = 1, BindingId = binding }, 8).BindingId);
    }

    [TestMethod]
    public void ControlAndPowerPids_RoundTrip()
    {
        Assert.IsTrue(RoundTrip(new PowerOnSelfTest.Set { Enabled = true }, 1).Enabled);
        Assert.AreEqual((byte)12, RoundTrip(new BurnIn.Set { Hours = 12 }, 1).Hours);
        Assert.AreEqual((byte)3, RoundTrip(new BurnIn.GetReply { HoursRemaining = 3 }, 1).HoursRemaining);
        Assert.AreEqual(PresetMergeMode.MergeModes.Ltp, RoundTrip(new PresetMergeMode.Set { MergeMode = PresetMergeMode.MergeModes.Ltp }, 1).MergeMode);

        var info = RoundTrip(new PresetInfo.GetReply { SplitTimesSupported = true, MaximumSceneNumber = 0x8001, MaximumStartupHoldTime = 0xFFFF }, 32);
        Assert.IsTrue(info.SplitTimesSupported);
        Assert.AreEqual((ushort)0x8001, info.MaximumSceneNumber);
        Assert.AreEqual((ushort)0xFFFF, info.MaximumStartupHoldTime);

        Assert.AreEqual((ushort)7, RoundTrip(new PresetStatus.Get { SceneNumber = 7 }, 2).SceneNumber);
        var status = RoundTrip(new PresetStatus.GetReply { SceneNumber = 7, UpFadeTime = 10, DownFadeTime = 20, WaitTime = 30, Programmed = PresetStatus.ProgrammedStates.ReadOnly }, 9);
        Assert.AreEqual((ushort)30, status.WaitTime);
        Assert.AreEqual(PresetStatus.ProgrammedStates.ReadOnly, status.Programmed);
        Assert.IsTrue(RoundTrip(new PresetStatus.Set { SceneNumber = 7, ClearPreset = true }, 9).ClearPreset);
    }

    [TestMethod]
    public void DmxFailAndStartupModes_RoundTrip()
    {
        var fail = RoundTrip(new DmxFailMode.Set { SceneNumber = 2, LossOfSignalDelay = 0xFFFF, HoldTime = 50, Level = 255 }, 7);
        Assert.AreEqual((ushort)0xFFFF, fail.LossOfSignalDelay);
        Assert.AreEqual((byte)255, fail.Level);

        var startup = RoundTrip(new DmxStartupMode.GetReply { SceneNumber = 3, StartupDelay = 40, HoldTime = 0xFFFF, Level = 128 }, 7);
        Assert.AreEqual((ushort)40, startup.StartupDelay);
        Assert.AreEqual((ushort)0xFFFF, startup.HoldTime);
    }

    [TestMethod]
    public void LockPids_RoundTrip()
    {
        Assert.AreEqual((ushort)1234, RoundTrip(new LockPin.GetReply { PinCode = 1234 }, 2).PinCode);

        var pin = RoundTrip(new LockPin.Set { NewPinCode = 4321, CurrentPinCode = 1234 }, 4);
        Assert.AreEqual((ushort)4321, pin.NewPinCode);
        Assert.AreEqual((ushort)1234, pin.CurrentPinCode);

        var state = RoundTrip(new LockState.Set { PinCode = 1234, NewLockState = 2 }, 3);
        Assert.AreEqual((byte)2, state.NewLockState);
        Assert.AreEqual((byte)3, RoundTrip(new LockState.GetReply { CurrentLockState = 1, LockStateCount = 3 }, 2).LockStateCount);

        var description = RoundTrip(new LockStateDescription.GetReply { LockState = 1, Description = "Locked" }, 7);
        Assert.AreEqual("Locked", description.Description);
    }

    [TestMethod]
    public void DimmerPids_RoundTrip()
    {
        var info = RoundTrip(new DimmerInfo.GetReply { MaximumLevelUpperLimit = 0xFFFF, NumberOfSupportedCurves = 4, LevelsResolution = 16, MinimumLevelSplitLevelsSupported = true }, 11);
        Assert.AreEqual((ushort)0xFFFF, info.MaximumLevelUpperLimit);
        Assert.AreEqual((byte)16, info.LevelsResolution);
        Assert.IsTrue(info.MinimumLevelSplitLevelsSupported);

        var minimum = RoundTrip(new MinimumLevel.Set { MinimumLevelIncreasing = 100, MinimumLevelDecreasing = 90, OnBelowMinimum = true }, 5);
        Assert.AreEqual((ushort)90, minimum.MinimumLevelDecreasing);
        Assert.IsTrue(minimum.OnBelowMinimum);

        Assert.AreEqual((ushort)60000, RoundTrip(new MaximumLevel.Set { Level = 60000 }, 2).Level);

        Assert.AreEqual((byte)4, RoundTrip(new Curve.GetReply { CurrentCurve = 2, CurveCount = 4 }, 2).CurveCount);
        Assert.AreEqual((byte)3, RoundTrip(new Curve.Set { CurveNumber = 3 }, 1).CurveNumber);
        Assert.AreEqual("Square law", RoundTrip(new CurveDescription.GetReply { CurveNumber = 2, Description = "Square law" }, 11).Description);

        Assert.AreEqual((byte)5, RoundTrip(new OutputResponseTime.GetReply { CurrentSetting = 1, SettingCount = 5 }, 2).SettingCount);
        Assert.AreEqual((byte)2, RoundTrip(new OutputResponseTime.Set { Setting = 2 }, 1).Setting);
        Assert.AreEqual("Fast", RoundTrip(new OutputResponseTimeDescription.GetReply { Setting = 1, Description = "Fast" }, 5).Description);

        Assert.AreEqual((byte)2, RoundTrip(new ModulationFrequency.Set { Setting = 2 }, 1).Setting);
        var frequency = RoundTrip(new ModulationFrequencyDescription.GetReply { Setting = 1, Frequency = 25000, Description = "25 kHz" }, 11);
        Assert.AreEqual(25000u, frequency.Frequency);
        Assert.AreEqual("25 kHz", frequency.Description);
    }
}
