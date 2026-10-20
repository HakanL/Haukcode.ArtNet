using System.Diagnostics;
using Haukcode.ArtNet.Packets;

namespace Haukcode.ArtNet.TimeCodeGenerator;

/// <summary>
/// Free-running SMPTE timecode clock that hands one <see cref="ArtTimeCodePacket"/> per
/// frame to a send callback from its own thread. Has no network dependency of its own, so
/// the same class can be driven by a raw socket (this CLI) or by an ArtNetClient.
///
/// Frame k is due at <c>anchor + k / fps</c>, computed from the exact frame rate (30000/1001
/// for drop frame) rather than a rounded per-frame period, so the clock never accumulates drift and, if
/// the host stalls for longer than a frame, it skips ahead rather than bursting so the
/// timecode stays aligned with wall time (like a real LTC source). Pause and hold freeze the
/// clock instead: the anchor slides with wall time and the count resumes where it stopped.
/// </summary>
public sealed class ArtTimeCodeGenerator : IDisposable
{
    private readonly object sync = new();
    private readonly Action<ArtTimeCodePacket> send;
    private readonly ArtTimeCodePacket packet = new();
    private readonly Random random = new();
    private readonly TimeCode start;

    private Thread? thread;

    // One per run, so a thread that outlives Stop's join timeout stays stopped after a new Start
    private CancellationTokenSource? stopSource;

    private ArtTimeCodeTypes type;
    private long frameRateNumerator;
    private long frameRateDenominator;
    private long anchorTicks;
    private long frameBase;
    private long lastSentIndex = -1;
    private bool seekPending;
    private bool paused;
    private bool hold;

    private long packetsSent;
    private long packetsDropped;
    private long sendErrors;
    private Exception? lastError;
    private double lateMaxMs;
    private double lateSumMs;
    private double lateSumSquaredMs;
    private long lateSamples;

    public ArtTimeCodeGenerator(TimeCode start, byte streamId, Action<ArtTimeCodePacket> send)
    {
        this.start = start;
        this.send = send;
        StreamId = streamId;

        this.type = start.Type;
        (this.frameRateNumerator, this.frameRateDenominator) = FrameRate(start.Type);
        this.frameBase = start.FrameNumber;
    }

    public byte StreamId { get; set; }

    /// <summary>Uniform random offset applied to each frame's send moment, plus/minus this many ms.</summary>
    public double JitterMs { get; set; }

    /// <summary>Percentage of frames to silently skip, to simulate packet loss.</summary>
    public double DropPercent { get; set; }

    public bool IsRunning => this.thread != null && this.stopSource?.IsCancellationRequested == false;

    public ArtTimeCodeTypes Type
    {
        get { lock (this.sync) return this.type; }
    }

    /// <summary>
    /// The frame most recently handed to the send callback. Right after a seek (or before the
    /// first frame) it is the frame that will go out next, so a stopped generator shows the
    /// value it is parked on.
    /// </summary>
    public TimeCode Current
    {
        get
        {
            lock (this.sync)
            {
                long index = this.seekPending ? this.lastSentIndex + 1 : Math.Max(this.lastSentIndex, 0);

                return TimeCode.FromFrameNumber(this.frameBase + index, this.type);
            }
        }
    }

    /// <summary>Stop sending packets entirely; the clock freezes.</summary>
    public bool Paused
    {
        get { lock (this.sync) return this.paused; }
        set { lock (this.sync) this.paused = value; }
    }

    /// <summary>Keep sending, but repeat the current frame; the clock freezes.</summary>
    public bool Hold
    {
        get { lock (this.sync) return this.hold; }
        set { lock (this.sync) this.hold = value; }
    }

    public void Start()
    {
        if (this.thread != null)
            throw new InvalidOperationException("Already started");

        lock (this.sync)
            this.anchorTicks = Stopwatch.GetTimestamp() - TicksForFrames(this.lastSentIndex + 1);

        var stopSource = new CancellationTokenSource();
        var stopToken = stopSource.Token;
        this.stopSource = stopSource;

        this.thread = new Thread(() => Run(stopToken))
        {
            Name = "ArtTimeCode generator",
            IsBackground = true,
            Priority = ThreadPriority.AboveNormal
        };
        this.thread.Start();
    }

    /// <summary>
    /// Stop the clock thread; <see cref="Start"/> can be called again afterwards and continues
    /// from the frame after the last one sent.
    /// </summary>
    public void Stop()
    {
        this.stopSource?.Cancel();
        this.thread?.Join(2_000);
        this.thread = null;
    }

    /// <summary>Move the clock by a number of frames (negative to go backwards); wraps at 24 h.</summary>
    public void Jump(long deltaFrames)
    {
        lock (this.sync)
            this.frameBase = Normalize(this.frameBase + deltaFrames, this.type);
    }

    /// <summary>Move the clock by a wall-time amount, rounded to whole frames.</summary>
    public void Jump(TimeSpan delta)
    {
        Jump((long)Math.Round(delta.TotalSeconds * TimeCode.RealFps(Type)));
    }

    /// <summary>The next frame sent will be <paramref name="timeCode"/>, switching frame rate if it differs.</summary>
    public void Seek(TimeCode timeCode)
    {
        lock (this.sync)
        {
            long nextIndex = this.lastSentIndex + 1;

            this.type = timeCode.Type;
            (this.frameRateNumerator, this.frameRateDenominator) = FrameRate(timeCode.Type);
            this.frameBase = Normalize(timeCode.FrameNumber - nextIndex, timeCode.Type);
            this.anchorTicks = Stopwatch.GetTimestamp() - TicksForFrames(nextIndex);
            this.seekPending = true;
        }
    }

    public void Reset() => Seek(this.start);

    /// <summary>Seek to the local time of day, like a generator's "clock" button.</summary>
    public void SeekToSystemClock()
    {
        var now = DateTime.Now;
        var type = Type;
        int frames = (int)(now.Millisecond / 1000.0 * TimeCode.NominalFps(type));

        var timeCode = new TimeCode(now.Hour, now.Minute, now.Second, frames, type);
        if (TimeCode.IsDropFrame(type) && now.Second == 0 && frames < 2 && now.Minute % 10 != 0)
            timeCode = timeCode with { Frames = 2 };

        Seek(timeCode);
    }

    /// <summary>Switch frame rate in place, carrying the current HH:MM:SS across.</summary>
    public void SetType(ArtTimeCodeTypes newType)
    {
        TimeCode next;
        lock (this.sync)
        {
            if (newType == this.type)
                return;

            next = TimeCode.FromFrameNumber(this.frameBase + this.lastSentIndex + 1, this.type);
        }

        int frames = Math.Min(next.Frames, TimeCode.NominalFps(newType) - 1);
        if (TimeCode.IsDropFrame(newType) && next.Seconds == 0 && frames < 2 && next.Minutes % 10 != 0)
            frames = 2;

        Seek(new TimeCode(next.Hours, next.Minutes, next.Seconds, frames, newType));
    }

    public Statistics GetStatistics()
    {
        lock (this.sync)
        {
            double mean = this.lateSamples > 0 ? this.lateSumMs / this.lateSamples : 0;
            double variance = this.lateSamples > 0 ? this.lateSumSquaredMs / this.lateSamples - mean * mean : 0;

            return new Statistics(
                PacketsSent: this.packetsSent,
                PacketsDropped: this.packetsDropped,
                SendErrors: this.sendErrors,
                LastError: this.lastError,
                LateMeanMs: mean,
                LateStdDevMs: Math.Sqrt(Math.Max(variance, 0)),
                LateMaxMs: this.lateMaxMs);
        }
    }

    public readonly record struct Statistics(
        long PacketsSent,
        long PacketsDropped,
        long SendErrors,
        Exception? LastError,
        double LateMeanMs,
        double LateStdDevMs,
        double LateMaxMs);

    private void Run(CancellationToken stopToken)
    {
        while (!stopToken.IsCancellationRequested)
        {
            long deadline;
            bool isPaused;
            long period;

            lock (this.sync)
            {
                period = TicksForFrames(1);

                if (this.paused || this.hold)
                {
                    // Freeze: slide the anchor so the next frame is always one period away
                    this.anchorTicks = Stopwatch.GetTimestamp() - TicksForFrames(this.lastSentIndex);
                }

                isPaused = this.paused;
                deadline = this.anchorTicks + TicksForFrames(this.lastSentIndex + 1);
            }

            if (isPaused)
            {
                // Idle in short steps so resume and stop are picked up promptly
                WaitUntil(Math.Min(deadline, Stopwatch.GetTimestamp() + MsToTicks(5)), stopToken);
                continue;
            }

            long sendAt = deadline + JitterTicks(period);
            WaitUntil(sendAt, stopToken);
            if (stopToken.IsCancellationRequested)
                break;

            long now = Stopwatch.GetTimestamp();

            // Decide the frame from the state as it is now, not as it was before the wait, so a
            // pause, hold or seek made meanwhile is honoured
            TimeCode timeCode;
            lock (this.sync)
            {
                if (this.paused)
                    continue;

                long index;
                if (this.hold)
                {
                    index = this.seekPending ? this.lastSentIndex + 1 : Math.Max(this.lastSentIndex, 0);
                }
                else
                {
                    index = this.lastSentIndex + 1;

                    // Stalled for more than a frame (debugger, scheduling hiccup): skip ahead
                    // instead of bursting, keeping the timecode aligned with wall time. Not right
                    // after a seek, whose target frame must go out first.
                    long dueIndex = FramesForTicks(now - this.anchorTicks);
                    if (!this.seekPending && dueIndex > index)
                        index = dueIndex;

                    this.lastSentIndex = index;
                    this.seekPending = false;
                }

                timeCode = TimeCode.FromFrameNumber(this.frameBase + index, this.type);
            }

            RecordLateness(now - sendAt);
            SendFrame(timeCode);
        }
    }

    private void SendFrame(TimeCode timeCode)
    {
        if (DropPercent > 0 && this.random.NextDouble() * 100 < DropPercent)
        {
            lock (this.sync)
                this.packetsDropped++;
            return;
        }

        this.packet.StreamId = StreamId;
        this.packet.Hours = (byte)timeCode.Hours;
        this.packet.Minutes = (byte)timeCode.Minutes;
        this.packet.Seconds = (byte)timeCode.Seconds;
        this.packet.Frames = (byte)timeCode.Frames;
        this.packet.Type = timeCode.Type;

        try
        {
            this.send(this.packet);

            lock (this.sync)
                this.packetsSent++;
        }
        catch (Exception ex)
        {
            lock (this.sync)
            {
                this.sendErrors++;
                this.lastError = ex;
            }
        }
    }

    private long JitterTicks(long period)
    {
        if (JitterMs <= 0)
            return 0;

        // Never reach into the neighbouring frames
        long maxOffset = Math.Min(MsToTicks(JitterMs), period / 2 - 1);

        return (long)((this.random.NextDouble() * 2 - 1) * maxOffset);
    }

    private void RecordLateness(long lateTicks)
    {
        double lateMs = Math.Max(lateTicks, 0) * 1000.0 / Stopwatch.Frequency;

        lock (this.sync)
        {
            this.lateSamples++;
            this.lateSumMs += lateMs;
            this.lateSumSquaredMs += lateMs * lateMs;
            if (lateMs > this.lateMaxMs)
                this.lateMaxMs = lateMs;
        }
    }

    /// <summary>
    /// Sleep in 1 ms steps until ~2 ms remain, then spin. On Windows the 1 ms sleep relies on
    /// the process having raised its timer resolution (see Program), otherwise it is ~15 ms.
    /// </summary>
    private static void WaitUntil(long targetTicks, CancellationToken stopToken)
    {
        long spinThreshold = MsToTicks(2);

        while (!stopToken.IsCancellationRequested)
        {
            long remaining = targetTicks - Stopwatch.GetTimestamp();
            if (remaining <= 0)
                return;

            if (remaining > spinThreshold)
                Thread.Sleep(1);
            else
                Thread.SpinWait(20);
        }
    }

    private static (long Numerator, long Denominator) FrameRate(ArtTimeCodeTypes type) =>
        TimeCode.IsDropFrame(type) ? (30000, 1001) : (TimeCode.NominalFps(type), 1);

    /// <summary>Stopwatch ticks spanned by a number of frames, exact apart from one final truncation.</summary>
    private long TicksForFrames(long frames) =>
        (long)((Int128)frames * Stopwatch.Frequency * this.frameRateDenominator / this.frameRateNumerator);

    private long FramesForTicks(long ticks) =>
        (long)((Int128)ticks * this.frameRateNumerator / ((Int128)Stopwatch.Frequency * this.frameRateDenominator));

    private static long MsToTicks(double ms) => (long)(ms * Stopwatch.Frequency / 1000.0);

    private static long Normalize(long frameNumber, ArtTimeCodeTypes type)
    {
        long framesPerDay = TimeCode.FramesPerDay(type);
        frameNumber %= framesPerDay;

        return frameNumber < 0 ? frameNumber + framesPerDay : frameNumber;
    }

    public void Dispose() => Stop();
}
