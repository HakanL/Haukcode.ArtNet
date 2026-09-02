using Haukcode.ArtNet.TimeCodeGenerator;

namespace Haukcode.ArtNet.Tests;

[TestClass]
public class TimeCodeTests
{
    [TestMethod]
    [DataRow(ArtTimeCodeTypes.Film, 24, 2_073_600L)]
    [DataRow(ArtTimeCodeTypes.Ebu, 25, 2_160_000L)]
    [DataRow(ArtTimeCodeTypes.DropFrame, 30, 2_589_408L)]
    [DataRow(ArtTimeCodeTypes.Smpte, 30, 2_592_000L)]
    public void NominalFpsAndFramesPerDay(ArtTimeCodeTypes type, int fps, long framesPerDay)
    {
        Assert.AreEqual(fps, TimeCode.NominalFps(type));
        Assert.AreEqual(framesPerDay, TimeCode.FramesPerDay(type));
    }

    [TestMethod]
    public void DropFrame_RealFpsIs2997()
    {
        Assert.AreEqual(29.97, TimeCode.RealFps(ArtTimeCodeTypes.DropFrame), 0.001);
        Assert.AreEqual(30, TimeCode.RealFps(ArtTimeCodeTypes.Smpte));
    }

    [TestMethod]
    [DataRow(0L, "00:00:00;00")]
    [DataRow(1799L, "00:00:59;29")]
    [DataRow(1800L, "00:01:00;02")]
    [DataRow(17981L, "00:09:59;29")]
    [DataRow(17982L, "00:10:00;00")]
    [DataRow(19782L, "00:11:00;02")]
    [DataRow(107892L, "01:00:00;00")]
    [DataRow(2_589_407L, "23:59:59;29")]
    public void DropFrame_KnownFrameNumbers(long frameNumber, string expected)
    {
        var timeCode = TimeCode.FromFrameNumber(frameNumber, ArtTimeCodeTypes.DropFrame);

        Assert.AreEqual(expected, timeCode.ToString());
        Assert.AreEqual(frameNumber, timeCode.FrameNumber);
    }

    [TestMethod]
    [DataRow(ArtTimeCodeTypes.Film)]
    [DataRow(ArtTimeCodeTypes.Ebu)]
    [DataRow(ArtTimeCodeTypes.DropFrame)]
    [DataRow(ArtTimeCodeTypes.Smpte)]
    public void FrameNumber_RoundTripsEveryFrameOfTheDay(ArtTimeCodeTypes type)
    {
        long framesPerDay = TimeCode.FramesPerDay(type);
        int fps = TimeCode.NominalFps(type);
        var previous = TimeCode.FromFrameNumber(framesPerDay - 1, type);

        for (long frameNumber = 0; frameNumber < framesPerDay; frameNumber++)
        {
            var timeCode = TimeCode.FromFrameNumber(frameNumber, type);

            Assert.AreEqual(frameNumber, timeCode.FrameNumber, $"round trip of {timeCode}");
            Assert.IsTrue(timeCode.Frames < fps, $"{timeCode} frames out of range");

            if (TimeCode.IsDropFrame(type))
            {
                bool dropped = timeCode.Seconds == 0 && timeCode.Frames < 2 && timeCode.Minutes % 10 != 0;
                Assert.IsFalse(dropped, $"{timeCode} is a dropped frame number");
            }

            // Consecutive frame numbers must count up by exactly one displayed frame,
            // wrapping at the end of each second (and skipping dropped numbers).
            bool sameSecond = timeCode.Hours == previous.Hours && timeCode.Minutes == previous.Minutes && timeCode.Seconds == previous.Seconds;
            if (sameSecond)
                Assert.AreEqual(previous.Frames + 1, timeCode.Frames, $"{previous} -> {timeCode}");
            else
                Assert.IsTrue(timeCode.Frames <= 2, $"{previous} -> {timeCode}");

            previous = timeCode;
        }
    }

    [TestMethod]
    public void FromFrameNumber_WrapsAtMidnight()
    {
        long framesPerDay = TimeCode.FramesPerDay(ArtTimeCodeTypes.Smpte);

        Assert.AreEqual("00:00:00:05", TimeCode.FromFrameNumber(framesPerDay + 5, ArtTimeCodeTypes.Smpte).ToString());
        Assert.AreEqual("23:59:59:29", TimeCode.FromFrameNumber(-1, ArtTimeCodeTypes.Smpte).ToString());
    }

    [TestMethod]
    [DataRow("01:02:03:04", ArtTimeCodeTypes.Smpte, 1, 2, 3, 4)]
    [DataRow("1:2:3:4", ArtTimeCodeTypes.Smpte, 1, 2, 3, 4)]
    [DataRow("01:02:03", ArtTimeCodeTypes.Smpte, 1, 2, 3, 0)]
    [DataRow("01:02", ArtTimeCodeTypes.Smpte, 1, 2, 0, 0)]
    [DataRow("00:59:50;02", ArtTimeCodeTypes.DropFrame, 0, 59, 50, 2)]
    [DataRow("00:01:00;02", ArtTimeCodeTypes.DropFrame, 0, 1, 0, 2)]
    [DataRow("00:10:00;00", ArtTimeCodeTypes.DropFrame, 0, 10, 0, 0)]
    [DataRow("23:59:59.23", ArtTimeCodeTypes.Film, 23, 59, 59, 23)]
    public void Parse_AcceptsCommonForms(string text, ArtTimeCodeTypes type, int hours, int minutes, int seconds, int frames)
    {
        var timeCode = TimeCode.Parse(text, type);

        Assert.AreEqual(new TimeCode(hours, minutes, seconds, frames, type), timeCode);
    }

    [TestMethod]
    [DataRow("24:00:00:00", ArtTimeCodeTypes.Smpte)]
    [DataRow("00:60:00:00", ArtTimeCodeTypes.Smpte)]
    [DataRow("00:00:60:00", ArtTimeCodeTypes.Smpte)]
    [DataRow("00:00:00:30", ArtTimeCodeTypes.Smpte)]
    [DataRow("00:00:00:25", ArtTimeCodeTypes.Ebu)]
    [DataRow("00:00:00:24", ArtTimeCodeTypes.Film)]
    [DataRow("00:01:00;00", ArtTimeCodeTypes.DropFrame)]
    [DataRow("00:01:00;01", ArtTimeCodeTypes.DropFrame)]
    [DataRow("abc", ArtTimeCodeTypes.Smpte)]
    [DataRow("1", ArtTimeCodeTypes.Smpte)]
    [DataRow("1:2:3:4:5", ArtTimeCodeTypes.Smpte)]
    [DataRow("-1:00:00:00", ArtTimeCodeTypes.Smpte)]
    public void Parse_RejectsInvalid(string text, ArtTimeCodeTypes type)
    {
        Assert.IsFalse(TimeCode.TryParse(text, type, out _, out string? error));
        Assert.IsNotNull(error);
        Assert.ThrowsExactly<FormatException>(() => TimeCode.Parse(text, type));
    }

    [TestMethod]
    public void ToString_UsesSemicolonForDropFrame()
    {
        Assert.AreEqual("01:02:03:04", new TimeCode(1, 2, 3, 4, ArtTimeCodeTypes.Smpte).ToString());
        Assert.AreEqual("01:02:03;04", new TimeCode(1, 2, 3, 4, ArtTimeCodeTypes.DropFrame).ToString());
    }
}
