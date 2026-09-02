using System.Diagnostics;
using Haukcode.ArtNet.Packets;

namespace Haukcode.ArtNet.TimeCodeGenerator;

/// <summary>
/// Free-running SMPTE timecode clock that hands one <see cref="ArtTimeCodePacket"/> per
/// frame to a send callback from its own thread. Has no network dependency of its own, so
/// the same class can be driven by a raw socket (this CLI) or by an ArtNetClient.
///
/// Frame k is due at <c>anchor + k * period</c>; the clock never accumulates drift and, if
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
    private volatile bool stopRequested;

    private ArtTimeCodeTypes type;
    private long periodTicks;
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
        this.periodTicks = PeriodTicks(start.Type);
        this.frameBase = start.FrameNumber;
    }

    public byte StreamId { get; set; }

    /// <summary>Uniform random offset applied to each frame's send moment, plus/minus this many ms.</summary>
    public double JitterMs { get; set; }

    /// <summary>Percentage of frames to silently skip, to simulate packet loss.</summary>
    public double DropPercent { get; set; }

    public bool IsRunning => this.thread != null && !this.stopRequested;

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

        this.anchorTicks = Stopwatch.GetTimestamp();

        this.thread = new Thread(Run)
        {
            Name = "ArtTimeCode generator",
            IsBackground = true,
            Priority = ThreadPriority.AboveNormal
        };
        this.thread.Start();
    }

    public void Stop()
    {
        this.stopRequested = true;
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
            this.periodTicks = PeriodTicks(timeCode.Type);
            this.frameBase = Normalize(timeCode.FrameNumber - nextIndex, timeCode.Type);
            this.anchorTicks = Stopwatch.GetTimestamp() - nextIndex * this.periodTicks;
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

    private void Run()
    {
        while (!this.stopRequested)
        {
            long deadline;
            bool isPaused;
            bool isHold;
            long period;

            lock (this.sync)
            {
                period = this.periodTicks;

                if (this.paused || this.hold)
                {
                    // Freeze: slide the anchor so the next frame is always one period away
                    this.anchorTicks = Stopwatch.GetTimestamp() - this.lastSentIndex * period;
                }

                isPaused = this.paused;
                isHold = this.hold;
                deadline = this.anchorTicks + (this.lastSentIndex + 1) * period;
            }

            if (isPaused)
            {
                // Idle in short steps so resume and stop are picked up promptly
                WaitUntil(Math.Min(deadline, Stopwatch.GetTimestamp() + MsToTicks(5)));
                continue;
            }

            long sendAt = deadline + JitterTicks(period);
            WaitUntil(sendAt);
            if (this.stopRequested)
                break;

            long now = Stopwatch.GetTimestamp();
            RecordLateness(now - sendAt);

            long index;
            lock (this.sync)
            {
                if (isHold)
                {
                    index = this.seekPending ? this.lastSentIndex + 1 : Math.Max(this.lastSentIndex, 0);
                }
                else
                {
                    index = this.lastSentIndex + 1;

                    // Stalled for more than a frame (debugger, scheduling hiccup): skip ahead
                    // instead of bursting, keeping the timecode aligned with wall time
                    long dueIndex = (now - this.anchorTicks) / period;
                    if (dueIndex > index)
                        index = dueIndex;

                    this.lastSentIndex = index;
                    this.seekPending = false;
                }
            }

            SendFrame(index);
        }
    }

    private void SendFrame(long index)
    {
        TimeCode timeCode;
        lock (this.sync)
            timeCode = TimeCode.FromFrameNumber(this.frameBase + index, this.type);

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
    private void WaitUntil(long targetTicks)
    {
        long spinThreshold = MsToTicks(2);

        while (!this.stopRequested)
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

    private static long PeriodTicks(ArtTimeCodeTypes type) =>
        (long)Math.Round(Stopwatch.Frequency / TimeCode.RealFps(type));

    private static long MsToTicks(double ms) => (long)(ms * Stopwatch.Frequency / 1000.0);

    private static long Normalize(long frameNumber, ArtTimeCodeTypes type)
    {
        long framesPerDay = TimeCode.FramesPerDay(type);
        frameNumber %= framesPerDay;

        return frameNumber < 0 ? frameNumber + framesPerDay : frameNumber;
    }

    public void Dispose() => Stop();
}
