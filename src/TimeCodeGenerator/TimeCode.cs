namespace Haukcode.ArtNet.TimeCodeGenerator;

/// <summary>
/// SMPTE timecode value (HH:MM:SS:FF) for one of the four Art-Net frame-rate types,
/// with conversion to and from a linear frame number since midnight. Drop-frame
/// (29.97) follows SMPTE 12M: frame numbers 0 and 1 are skipped at the start of
/// every minute except minutes 0, 10, 20, 30, 40 and 50.
/// </summary>
public readonly record struct TimeCode(int Hours, int Minutes, int Seconds, int Frames, ArtTimeCodeTypes Type)
{
    private const int DropFramesPerMinute = 2;
    private const long DropFrameFramesPerMinute = 30 * 60 - DropFramesPerMinute;          // 1798
    private const long DropFrameFramesPerTenMinutes = 30 * 60 * 10 - DropFramesPerMinute * 9; // 17982

    /// <summary>Frames counted per nominal second (30 for drop-frame).</summary>
    public static int NominalFps(ArtTimeCodeTypes type) => type switch
    {
        ArtTimeCodeTypes.Film => 24,
        ArtTimeCodeTypes.Ebu => 25,
        ArtTimeCodeTypes.DropFrame => 30,
        ArtTimeCodeTypes.Smpte => 30,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Unknown timecode type")
    };

    /// <summary>Actual frames transmitted per real-time second.</summary>
    public static double RealFps(ArtTimeCodeTypes type) =>
        type == ArtTimeCodeTypes.DropFrame ? 30000.0 / 1001.0 : NominalFps(type);

    public static bool IsDropFrame(ArtTimeCodeTypes type) => type == ArtTimeCodeTypes.DropFrame;

    /// <summary>Number of distinct frame numbers in 24 hours.</summary>
    public static long FramesPerDay(ArtTimeCodeTypes type) =>
        IsDropFrame(type)
            ? 24 * 6 * DropFrameFramesPerTenMinutes   // 2,589,408
            : 24L * 3600 * NominalFps(type);

    /// <summary>Linear frame number since 00:00:00:00, drop-frame adjusted.</summary>
    public long FrameNumber
    {
        get
        {
            int fps = NominalFps(Type);
            long frameNumber = ((Hours * 60L + Minutes) * 60 + Seconds) * fps + Frames;

            if (IsDropFrame(Type))
            {
                long totalMinutes = Hours * 60L + Minutes;
                frameNumber -= DropFramesPerMinute * (totalMinutes - totalMinutes / 10);
            }

            return frameNumber;
        }
    }

    public static TimeCode FromFrameNumber(long frameNumber, ArtTimeCodeTypes type)
    {
        long framesPerDay = FramesPerDay(type);
        frameNumber %= framesPerDay;
        if (frameNumber < 0)
            frameNumber += framesPerDay;

        int fps = NominalFps(type);

        if (IsDropFrame(type))
        {
            // Re-insert the dropped frame numbers so plain division below lands on the
            // displayed HH:MM:SS:FF. Every 10-minute block hides 18 numbers; within a block
            // each full minute after the first hides 2.
            long tenMinuteBlocks = frameNumber / DropFrameFramesPerTenMinutes;
            long remainder = frameNumber % DropFrameFramesPerTenMinutes;

            frameNumber += DropFramesPerMinute * 9 * tenMinuteBlocks;
            if (remainder >= DropFramesPerMinute)
                frameNumber += DropFramesPerMinute * ((remainder - DropFramesPerMinute) / DropFrameFramesPerMinute);
        }

        int frames = (int)(frameNumber % fps);
        long totalSeconds = frameNumber / fps;

        return new TimeCode(
            Hours: (int)(totalSeconds / 3600 % 24),
            Minutes: (int)(totalSeconds / 60 % 60),
            Seconds: (int)(totalSeconds % 60),
            Frames: frames,
            Type: type);
    }

    /// <summary>
    /// Parse "HH:MM:SS:FF" (also accepts ';' or '.' before the frames, and omitted
    /// frames/seconds). Throws <see cref="FormatException"/> with a readable message
    /// when out of range or, for drop-frame, when naming a dropped frame number.
    /// </summary>
    public static TimeCode Parse(string text, ArtTimeCodeTypes type)
    {
        if (!TryParse(text, type, out var result, out string? error))
            throw new FormatException(error);

        return result;
    }

    public static bool TryParse(string text, ArtTimeCodeTypes type, out TimeCode result, out string? error)
    {
        result = default;
        error = null;

        string[] parts = text.Trim().Split([':', ';', '.'], StringSplitOptions.None);
        if (parts.Length < 2 || parts.Length > 4)
        {
            error = $"'{text}' is not a timecode, expected HH:MM:SS:FF";
            return false;
        }

        var values = new int[4];
        for (int i = 0; i < parts.Length; i++)
        {
            if (!int.TryParse(parts[i], out values[i]) || values[i] < 0)
            {
                error = $"'{text}' is not a timecode, expected HH:MM:SS:FF";
                return false;
            }
        }

        int hours = values[0], minutes = values[1], seconds = values[2], frames = values[3];
        int fps = NominalFps(type);

        if (hours > 23 || minutes > 59 || seconds > 59 || frames >= fps)
        {
            error = $"'{text}' is out of range for {Describe(type)} (max 23:59:59:{fps - 1:00})";
            return false;
        }

        if (IsDropFrame(type) && seconds == 0 && frames < DropFramesPerMinute && minutes % 10 != 0)
        {
            error = $"'{text}' names a dropped frame; drop-frame skips frames 00 and 01 at the start of every minute not divisible by 10";
            return false;
        }

        result = new TimeCode(hours, minutes, seconds, frames, type);
        return true;
    }

    public static string Describe(ArtTimeCodeTypes type) => type switch
    {
        ArtTimeCodeTypes.Film => "24 fps Film",
        ArtTimeCodeTypes.Ebu => "25 fps EBU",
        ArtTimeCodeTypes.DropFrame => "29.97 fps drop-frame",
        ArtTimeCodeTypes.Smpte => "30 fps SMPTE",
        _ => type.ToString()
    };

    public override string ToString()
    {
        char frameSeparator = IsDropFrame(Type) ? ';' : ':';

        return $"{Hours:00}:{Minutes:00}:{Seconds:00}{frameSeparator}{Frames:00}";
    }
}
