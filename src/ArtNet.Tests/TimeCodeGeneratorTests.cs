using Haukcode.ArtNet.Packets;
using Haukcode.ArtNet.TimeCodeGenerator;

namespace Haukcode.ArtNet.Tests;

/// <summary>
/// Wall-clock driven, so counts use wide tolerances; the ordering and content checks are exact.
/// </summary>
[TestClass]
public class TimeCodeGeneratorTests
{
    private static TimeCode Capture(ArtTimeCodePacket packet) =>
        new(packet.Hours, packet.Minutes, packet.Seconds, packet.Frames, packet.Type);

    private static List<TimeCode> Run(TimeCode start, int milliseconds, Action<ArtTimeCodeGenerator>? during = null, byte streamId = 0)
    {
        var frames = new List<TimeCode>();

        using var generator = new ArtTimeCodeGenerator(start, streamId, packet =>
        {
            Assert.AreEqual(streamId, packet.StreamId);
            lock (frames)
                frames.Add(Capture(packet));
        });

        generator.Start();
        during?.Invoke(generator);
        Thread.Sleep(milliseconds);
        generator.Stop();

        return frames;
    }

    [TestMethod]
    public void SendsConsecutiveFramesFromStart()
    {
        var start = TimeCode.Parse("00:59:59:20", ArtTimeCodeTypes.Smpte);

        var frames = Run(start, 600, streamId: 7);

        Assert.IsTrue(frames.Count is >= 10 and <= 24, $"expected ~18 frames in 600 ms at 30 fps, got {frames.Count}");
        Assert.AreEqual(start, frames[0]);

        for (int i = 1; i < frames.Count; i++)
            Assert.AreEqual(frames[i - 1].FrameNumber + 1, frames[i].FrameNumber, $"frame {i}: {frames[i - 1]} -> {frames[i]}");

        // Crossed the hour boundary
        Assert.IsTrue(frames.Any(x => x.Hours == 1 && x.Minutes == 0 && x.Seconds == 0));
    }

    [TestMethod]
    public void DropFrame_SkipsDroppedNumbersAtMinuteBoundary()
    {
        var start = TimeCode.Parse("00:00:59;25", ArtTimeCodeTypes.DropFrame);

        var frames = Run(start, 500);

        int boundary = frames.FindIndex(x => x.Minutes == 1);
        Assert.IsTrue(boundary > 0, "did not reach 00:01:00");
        Assert.AreEqual("00:00:59;29", frames[boundary - 1].ToString());
        Assert.AreEqual("00:01:00;02", frames[boundary].ToString());
    }

    [TestMethod]
    public void Pause_StopsSendingAndResumesWhereItLeftOff()
    {
        var start = TimeCode.Parse("00:00:00:00", ArtTimeCodeTypes.Smpte);
        int countWhenPaused = 0;
        int countAfterPause = 0;

        var frames = Run(start, 300, generator =>
        {
            Thread.Sleep(300);
            generator.Paused = true;
            Thread.Sleep(50);
            countWhenPaused = FrameCount(generator);
            Thread.Sleep(300);
            countAfterPause = FrameCount(generator);
            generator.Paused = false;
        });

        Assert.IsTrue(countWhenPaused > 0);
        Assert.AreEqual(countWhenPaused, countAfterPause, "frames were sent while paused");
        Assert.IsTrue(frames.Count > countAfterPause, "did not resume");

        for (int i = 1; i < frames.Count; i++)
            Assert.AreEqual(frames[i - 1].FrameNumber + 1, frames[i].FrameNumber, "resume must continue the count, not jump");
    }

    [TestMethod]
    public void Hold_RepeatsTheCurrentFrame()
    {
        var start = TimeCode.Parse("00:00:00:00", ArtTimeCodeTypes.Smpte);
        int countAtHold = 0;

        var frames = Run(start, 0, generator =>
        {
            Thread.Sleep(300);
            generator.Hold = true;
            Thread.Sleep(50);
            countAtHold = FrameCount(generator);
            Thread.Sleep(400);
        });

        Assert.IsTrue(frames.Count > countAtHold + 5, "hold must keep sending");

        var held = frames[^1];
        Assert.IsTrue(frames.Skip(countAtHold).All(x => x == held), "hold must repeat the same frame");
    }

    [TestMethod]
    public void Jump_MovesTheClockWithoutBreakingRate()
    {
        var start = TimeCode.Parse("00:00:00:00", ArtTimeCodeTypes.Smpte);

        var frames = Run(start, 0, generator =>
        {
            Thread.Sleep(200);
            generator.Jump(TimeSpan.FromSeconds(10));
            Thread.Sleep(200);
        });

        int jumpAt = frames.FindIndex(x => x.Seconds >= 10);
        Assert.IsTrue(jumpAt > 0);
        Assert.AreEqual(frames[jumpAt - 1].FrameNumber + 1 + 300, frames[jumpAt].FrameNumber);
        Assert.IsTrue(frames.Count > jumpAt + 2);
    }

    [TestMethod]
    public void Reset_ReturnsToStart()
    {
        var start = TimeCode.Parse("02:00:00:00", ArtTimeCodeTypes.Ebu);

        var frames = Run(start, 0, generator =>
        {
            Thread.Sleep(300);
            generator.Reset();
            Thread.Sleep(200);
        });

        Assert.AreEqual(2, frames.Count(x => x == start), "start frame should be sent twice");
    }

    [TestMethod]
    public void StopThenPlay_ParksOnStartAndResumesFromIt()
    {
        var start = TimeCode.Parse("00:59:50:00", ArtTimeCodeTypes.Smpte);
        TimeCode parkedOn = default;
        int sentWhileStopped = 0;

        var frames = Run(start, 0, generator =>
        {
            Thread.Sleep(200);

            // "Stop" as the CLI does it: pause, then rewind
            generator.Paused = true;
            generator.Reset();
            Thread.Sleep(100);
            int countAtStop = FrameCount(generator);
            Thread.Sleep(200);
            sentWhileStopped = FrameCount(generator) - countAtStop;
            parkedOn = generator.Current;

            generator.Paused = false;
            Thread.Sleep(150);
        });

        Assert.AreEqual(0, sentWhileStopped);
        Assert.AreEqual(start, parkedOn, "a stopped generator should display the value it will play from");
        Assert.AreEqual(2, frames.Count(x => x == start), "start frame sent at launch and again after stop/play");
        Assert.AreEqual(start.FrameNumber + 1, frames[frames.LastIndexOf(start) + 1].FrameNumber);
    }

    [TestMethod]
    public void SeekToSystemClock_LandsNearTimeOfDay()
    {
        var before = DateTime.Now;

        var frames = Run(TimeCode.Parse("00:00:00:00", ArtTimeCodeTypes.Ebu), 0, generator =>
        {
            Thread.Sleep(100);
            generator.SeekToSystemClock();
            Thread.Sleep(150);
        });

        var after = DateTime.Now;
        var jumped = frames.First(x => x.Hours == before.Hour || x.Hours == after.Hour);
        var jumpedTime = new TimeSpan(jumped.Hours, jumped.Minutes, jumped.Seconds);

        Assert.IsTrue(jumpedTime >= before.TimeOfDay - TimeSpan.FromSeconds(1) && jumpedTime <= after.TimeOfDay + TimeSpan.FromSeconds(1),
            $"{jumped} is not near {before:HH:mm:ss}");
    }

    [TestMethod]
    public void SetType_SwitchesRateAndKeepsTimeOfDay()
    {
        var start = TimeCode.Parse("01:02:03:00", ArtTimeCodeTypes.Smpte);

        var frames = Run(start, 0, generator =>
        {
            Thread.Sleep(200);
            generator.SetType(ArtTimeCodeTypes.Ebu);
            Thread.Sleep(200);
        });

        int switchAt = frames.FindIndex(x => x.Type == ArtTimeCodeTypes.Ebu);
        Assert.IsTrue(switchAt > 0);
        Assert.IsTrue(frames.Skip(switchAt).All(x => x.Type == ArtTimeCodeTypes.Ebu && x.Frames < 25));
        Assert.AreEqual((1, 2, 3), (frames[switchAt].Hours, frames[switchAt].Minutes, frames[switchAt].Seconds));
    }

    [TestMethod]
    public void DropPercent_SkipsFramesAndCountsThem()
    {
        int sent = 0;
        using var generator = new ArtTimeCodeGenerator(
            TimeCode.Parse("00:00:00:00", ArtTimeCodeTypes.Smpte),
            0,
            _ => sent++)
        {
            DropPercent = 100
        };

        generator.Start();
        Thread.Sleep(300);
        generator.Stop();

        var stats = generator.GetStatistics();
        Assert.AreEqual(0, sent);
        Assert.AreEqual(0, stats.PacketsSent);
        Assert.IsTrue(stats.PacketsDropped > 0);
    }

    [TestMethod]
    public void SendErrors_AreCountedNotThrown()
    {
        using var generator = new ArtTimeCodeGenerator(
            TimeCode.Parse("00:00:00:00", ArtTimeCodeTypes.Smpte),
            0,
            _ => throw new InvalidOperationException("boom"));

        generator.Start();
        Thread.Sleep(200);
        generator.Stop();

        var stats = generator.GetStatistics();
        Assert.IsTrue(stats.SendErrors > 0);
        Assert.AreEqual(0, stats.PacketsSent);
        Assert.AreEqual("boom", stats.LastError?.Message);
    }

    private static int FrameCount(ArtTimeCodeGenerator generator) => (int)generator.GetStatistics().PacketsSent;
}
