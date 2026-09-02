using Haukcode.ArtNet;
using Haukcode.ArtNet.TimeCodeGenerator;

namespace Haukcode.ArtNet.Tests;

[TestClass]
public class TimeCodeGeneratorScriptTests
{
    [TestMethod]
    public void Parse_TypicalChaseScript()
    {
        var steps = GeneratorScript.Parse(
            "play 14s; hold 3s; seek 01:00:08:00; play 6s",
            ArtTimeCodeTypes.Film);

        Assert.AreEqual(4, steps.Count);
        Assert.AreEqual(ScriptAction.Play, steps[0].Action);
        Assert.AreEqual(14, steps[0].DurationSeconds);
        Assert.AreEqual(ScriptAction.Hold, steps[1].Action);
        Assert.AreEqual(3, steps[1].DurationSeconds);
        Assert.AreEqual(ScriptAction.Seek, steps[2].Action);
        Assert.AreEqual(0, steps[2].DurationSeconds);
        Assert.AreEqual(TimeCode.Parse("01:00:08:00", ArtTimeCodeTypes.Film), steps[2].SeekTarget);
        Assert.AreEqual(6, steps[3].DurationSeconds);
    }

    [TestMethod]
    public void Parse_MillisecondsAndBareSeconds()
    {
        var steps = GeneratorScript.Parse("pause 500ms; wait 1.5; play 2s", ArtTimeCodeTypes.Smpte);

        Assert.AreEqual(0.5, steps[0].DurationSeconds);
        Assert.AreEqual(ScriptAction.Pause, steps[0].Action);
        Assert.AreEqual(1.5, steps[1].DurationSeconds);
        Assert.AreEqual(ScriptAction.Wait, steps[1].Action);
        Assert.AreEqual(2, steps[2].DurationSeconds);
    }

    [TestMethod]
    public void Parse_UnboundedPlayOnlyAsLastStep()
    {
        var steps = GeneratorScript.Parse("play 2s; play", ArtTimeCodeTypes.Smpte);

        Assert.IsFalse(steps[0].Unbounded);
        Assert.IsTrue(steps[1].Unbounded);
    }

    [TestMethod]
    public void Parse_UnboundedInTheMiddle_Throws()
    {
        var ex = Assert.ThrowsExactly<OptionException>(
            () => GeneratorScript.Parse("play; hold 1s", ArtTimeCodeTypes.Smpte));

        StringAssert.Contains(ex.Message, "no duration");
    }

    [TestMethod]
    public void Parse_Empty_Throws()
    {
        Assert.ThrowsExactly<OptionException>(() => GeneratorScript.Parse(" ; ; ", ArtTimeCodeTypes.Smpte));
    }

    [TestMethod]
    public void Parse_UnknownCommand_Throws()
    {
        var ex = Assert.ThrowsExactly<OptionException>(
            () => GeneratorScript.Parse("rewind 1s", ArtTimeCodeTypes.Smpte));

        StringAssert.Contains(ex.Message, "rewind");
    }

    [TestMethod]
    public void Apply_PlayHoldPauseSeek()
    {
        var start = TimeCode.Parse("00:59:50:00", ArtTimeCodeTypes.Film);
        using var generator = new ArtTimeCodeGenerator(start, 0, _ => { });

        GeneratorScript.Apply(generator, new ScriptStep(ScriptAction.Hold, 1, null));
        Assert.IsTrue(generator.Hold);
        Assert.IsFalse(generator.Paused);

        GeneratorScript.Apply(generator, new ScriptStep(ScriptAction.Pause, 1, null));
        Assert.IsTrue(generator.Paused);
        Assert.IsFalse(generator.Hold);

        GeneratorScript.Apply(generator, new ScriptStep(ScriptAction.Play, 1, null));
        Assert.IsFalse(generator.Paused);
        Assert.IsFalse(generator.Hold);

        var target = TimeCode.Parse("01:00:08:00", ArtTimeCodeTypes.Film);
        GeneratorScript.Apply(generator, new ScriptStep(ScriptAction.Seek, 0, target));
        Assert.AreEqual(target, generator.Current);
    }

    [TestMethod]
    public void ScriptRunner_SkipsInstantSeekThenPlays()
    {
        var start = TimeCode.Parse("00:59:50:00", ArtTimeCodeTypes.Film);
        using var generator = new ArtTimeCodeGenerator(start, 0, _ => { }) { Paused = true };

        var steps = GeneratorScript.Parse("seek 01:00:08:00; play 2s", ArtTimeCodeTypes.Film);
        var runner = new ScriptRunner(steps);
        runner.Start(generator);

        Assert.IsFalse(runner.Finished);
        Assert.IsFalse(generator.Paused);
        Assert.AreEqual(TimeCode.Parse("01:00:08:00", ArtTimeCodeTypes.Film), generator.Current);
        Assert.IsFalse(runner.Tick(generator, 1.9));
        Assert.IsTrue(runner.Tick(generator, 2.0));
        Assert.IsTrue(runner.Finished);
    }

    [TestMethod]
    public void ScriptRunner_HoldThenSeekThenPlay()
    {
        var start = TimeCode.Parse("00:00:00:00", ArtTimeCodeTypes.Smpte);
        using var generator = new ArtTimeCodeGenerator(start, 0, _ => { });

        var steps = GeneratorScript.Parse("hold 1s; seek 00:00:08:00; play 1s", ArtTimeCodeTypes.Smpte);
        var runner = new ScriptRunner(steps);
        runner.Start(generator);

        Assert.IsTrue(generator.Hold);
        Assert.IsFalse(runner.Tick(generator, 0.9));

        Assert.IsFalse(runner.Tick(generator, 1.0));
        Assert.IsFalse(generator.Hold);
        Assert.IsFalse(generator.Paused);
        Assert.AreEqual(TimeCode.Parse("00:00:08:00", ArtTimeCodeTypes.Smpte), generator.Current);

        Assert.IsTrue(runner.Tick(generator, 2.0));
    }

    [TestMethod]
    public void Options_ParseHoldAndScript()
    {
        var options = Options.Parse(["--hold", "--script", "play 14s; hold 3s", "--script", "play 6s"]);

        Assert.IsTrue(options.Hold);
        Assert.AreEqual("play 14s; hold 3s;play 6s", options.Script);
    }
}
