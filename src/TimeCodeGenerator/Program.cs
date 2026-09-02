using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using Haukcode.ArtNet.Packets;

namespace Haukcode.ArtNet.TimeCodeGenerator;

public static class Program
{
    private static readonly TimeSpan StatusInterval = TimeSpan.FromMilliseconds(250);

    public static int Main(string[] args)
    {
        Options options;
        try
        {
            options = Options.Parse(args);
        }
        catch (OptionException ex)
        {
            Console.Error.WriteLine(ex.Message);
            Console.Error.WriteLine();
            Console.Error.WriteLine(Options.Usage);
            return 2;
        }

        if (options.Help)
        {
            Console.WriteLine(Options.Usage);
            return 0;
        }

        var interfaces = Haukcode.Network.Utils.GetCommonInterfaces();

        if (options.ListInterfaces)
        {
            PrintInterfaces(interfaces);
            return 0;
        }

        IPAddress localAddress;
        IPAddress netMask;
        string adapterName;

        if (options.LocalAddress != null)
        {
            var match = interfaces.FirstOrDefault(x => x.IPAddress.Equals(options.LocalAddress));
            if (match.IPAddress == null)
            {
                Console.Error.WriteLine($"No local interface has the address {options.LocalAddress}. Available:");
                PrintInterfaces(interfaces);
                return 1;
            }

            (adapterName, _, localAddress, netMask) = match;
        }
        else
        {
            var first = Haukcode.Network.Utils.GetFirstBindAddress();
            if (first.IPAddress == null || first.NetMask == null)
            {
                Console.Error.WriteLine("No usable network interface found");
                return 1;
            }

            localAddress = first.IPAddress;
            netMask = first.NetMask;
            adapterName = interfaces.FirstOrDefault(x => x.IPAddress.Equals(localAddress)).AdapterName ?? "?";
        }

        if (!TimeCode.TryParse(options.Start, options.Type, out var start, out string? startError))
        {
            Console.Error.WriteLine(startError);
            return 2;
        }

        bool isBroadcast = options.Destination == null;
        var destination = new IPEndPoint(
            options.Destination ?? Haukcode.Network.Utils.GetBroadcastAddress(localAddress, netMask),
            options.Port);

        using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        socket.EnableBroadcast = true;
        Haukcode.Network.Utils.SetSocketOptions(socket);
        socket.Bind(new IPEndPoint(localAddress, 0));

        var buffer = new byte[ArtNetPacket.MAX_PACKET_SIZE];

        void Send(ArtTimeCodePacket packet)
        {
            int length = packet.WriteToBuffer(buffer);
            socket.SendTo(buffer.AsSpan(0, length), SocketFlags.None, destination);
        }

        IReadOnlyList<ScriptStep>? script = null;
        if (options.Script != null)
        {
            try
            {
                script = GeneratorScript.Parse(options.Script, options.Type);
            }
            catch (OptionException ex)
            {
                Console.Error.WriteLine(ex.Message);
                return 2;
            }
        }

        using var generator = new ArtTimeCodeGenerator(start, options.StreamId, Send)
        {
            JitterMs = options.JitterMs,
            DropPercent = options.DropPercent,
            Paused = options.Stopped,
            Hold = options.Hold
        };

        ScriptRunner? scriptRunner = null;
        if (script != null)
        {
            scriptRunner = new ScriptRunner(script);
            scriptRunner.Start(generator);
        }

        Console.WriteLine("Art-Net timecode generator");
        Console.WriteLine($"  Interface:   {localAddress} / {netMask} ({adapterName})");
        Console.WriteLine($"  Destination: {destination} ({(isBroadcast ? "broadcast" : "unicast")})");
        Console.WriteLine($"  Timecode:    {TimeCode.Describe(options.Type)}, start {start}, stream {options.StreamId}");
        if (options.JitterMs > 0 || options.DropPercent > 0)
            Console.WriteLine($"  Impairment:  jitter +/- {options.JitterMs} ms, drop {options.DropPercent}%");
        if (options.DurationSeconds > 0)
            Console.WriteLine($"  Duration:    {options.DurationSeconds} s");
        if (script != null)
            Console.WriteLine($"  Script:      {options.Script}");
        else if (options.Hold)
            Console.WriteLine("  Transport:   hold (repeating first frame)");

        bool interactive = !Console.IsInputRedirected;
        if (interactive)
        {
            Console.WriteLine("  Transport:   Space/P play-pause   S stop   R rewind   L locate   C clock   H hold   Q quit");
            Console.WriteLine("               +/- jump 10 s   ./, step one frame   1-4 rate (24/25/29.97/30)");
        }
        else if (options.Stopped && script == null)
        {
            Console.WriteLine("  Warning:     --stopped with no interactive console; nothing will ever be sent");
        }
        Console.WriteLine();

        using var timerResolution = TimerResolution.Raise();
        using var cancellation = new CancellationTokenSource();
        Console.CancelKeyPress += (_, e) =>
        {
            e.Cancel = true;
            cancellation.Cancel();
        };

        generator.Start();

        var runClock = Stopwatch.StartNew();
        var statusClock = Stopwatch.StartNew();

        while (!cancellation.IsCancellationRequested)
        {
            if (options.DurationSeconds > 0 && runClock.Elapsed.TotalSeconds >= options.DurationSeconds)
                break;

            if (scriptRunner != null && scriptRunner.Tick(generator, runClock.Elapsed.TotalSeconds))
                break;

            if (interactive && Console.KeyAvailable && HandleKey(Console.ReadKey(intercept: true), generator, start))
                break;

            if (!options.Quiet && statusClock.Elapsed >= StatusInterval)
            {
                WriteStatus(generator, start);
                statusClock.Restart();
            }

            Thread.Sleep(20);
        }

        generator.Stop();

        if (!options.Quiet)
            WriteStatus(generator, start);

        var stats = generator.GetStatistics();
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine($"Stopped at {generator.Current} after {runClock.Elapsed.TotalSeconds:0.0} s");
        Console.WriteLine($"  Packets: {stats.PacketsSent} sent, {stats.PacketsDropped} dropped by --drop, {stats.SendErrors} send errors");
        Console.WriteLine($"  Scheduling lateness: mean {stats.LateMeanMs:0.000} ms, std dev {stats.LateStdDevMs:0.000} ms, max {stats.LateMaxMs:0.000} ms");
        if (stats.LastError != null)
            Console.WriteLine($"  Last send error: {stats.LastError.Message}");

        return stats.SendErrors > 0 && stats.PacketsSent == 0 ? 1 : 0;
    }

    /// <summary>Returns true when the key asks to quit.</summary>
    private static bool HandleKey(ConsoleKeyInfo key, ArtTimeCodeGenerator generator, TimeCode start)
    {
        switch (key.Key)
        {
            case ConsoleKey.Q:
            case ConsoleKey.Escape:
                return true;

            case ConsoleKey.Spacebar:
            case ConsoleKey.P:
                generator.Paused = !generator.Paused;
                return false;

            case ConsoleKey.S:
                generator.Paused = true;
                generator.Reset();
                return false;

            case ConsoleKey.R:
            case ConsoleKey.Home:
                generator.Reset();
                return false;

            case ConsoleKey.L:
                Locate(generator, start);
                return false;

            case ConsoleKey.C:
                generator.SeekToSystemClock();
                return false;

            case ConsoleKey.H:
                generator.Hold = !generator.Hold;
                return false;
        }

        switch (key.KeyChar)
        {
            case '+':
            case '=':
                generator.Jump(TimeSpan.FromSeconds(10));
                break;
            case '-':
            case '_':
                generator.Jump(TimeSpan.FromSeconds(-10));
                break;
            case '.':
            case '>':
                generator.Jump(1);
                break;
            case ',':
            case '<':
                generator.Jump(-1);
                break;
            case '1':
                generator.SetType(ArtTimeCodeTypes.Film);
                break;
            case '2':
                generator.SetType(ArtTimeCodeTypes.Ebu);
                break;
            case '3':
                generator.SetType(ArtTimeCodeTypes.DropFrame);
                break;
            case '4':
                generator.SetType(ArtTimeCodeTypes.Smpte);
                break;
        }

        return false;
    }

    /// <summary>
    /// Prompt for a timecode on its own line (the status line is suspended while typing) and
    /// continue from there. Empty input cancels.
    /// </summary>
    private static void Locate(ArtTimeCodeGenerator generator, TimeCode start)
    {
        var type = generator.Type;

        Console.WriteLine();
        Console.Write($"Locate to (HH:MM:SS:FF, {TimeCode.Describe(type)}, Enter to cancel): ");
        string? input = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(input))
            return;

        if (TimeCode.TryParse(input, type, out var target, out string? error))
            generator.Seek(target);
        else
            Console.WriteLine(error);
    }

    private static void WriteStatus(ArtTimeCodeGenerator generator, TimeCode start)
    {
        var stats = generator.GetStatistics();

        string state = generator.Paused ? (generator.Current == start ? "STOPPED" : "PAUSED")
            : generator.Hold ? "HOLD (repeating)"
            : "PLAYING";

        string line = $"{generator.Current}  {TimeCode.Describe(generator.Type),-21} stream {generator.StreamId,-3} {state,-16} " +
            $"sent {stats.PacketsSent}  late avg {stats.LateMeanMs:0.00} ms max {stats.LateMaxMs:0.0} ms";

        if (stats.SendErrors > 0)
            line += $"  ERRORS {stats.SendErrors}";

        int width = 160;
        try
        {
            width = Math.Max(Console.WindowWidth - 1, 20);
        }
        catch (IOException)
        {
        }

        if (line.Length > width)
            line = line[..width];

        Console.Write("\r" + line.PadRight(width));
    }

    private static void PrintInterfaces(IList<(string AdapterName, string Description, IPAddress IPAddress, IPAddress NetMask)> interfaces)
    {
        if (interfaces.Count == 0)
        {
            Console.WriteLine("  (none)");
            return;
        }

        foreach (var (adapterName, description, address, netMask) in interfaces)
        {
            var broadcast = Haukcode.Network.Utils.GetBroadcastAddress(address, netMask);
            Console.WriteLine($"  {address,-15} / {netMask,-15} broadcast {broadcast,-15} {adapterName} ({description})");
        }
    }

    /// <summary>
    /// Windows sleeps in ~15.6 ms slices unless the process asks for a finer timer, which
    /// would put the frame pacing off by up to half a frame. Requesting 1 ms makes the 1 ms
    /// sleeps in the generator's wait loop actually 1 ms.
    /// </summary>
    private sealed class TimerResolution : IDisposable
    {
        private const uint PeriodMs = 1;
        private readonly bool raised;

        private TimerResolution(bool raised)
        {
            this.raised = raised;
        }

        public static TimerResolution Raise()
        {
            if (!OperatingSystem.IsWindows())
                return new TimerResolution(raised: false);

            return new TimerResolution(raised: timeBeginPeriod(PeriodMs) == 0);
        }

        public void Dispose()
        {
            if (this.raised)
                timeEndPeriod(PeriodMs);
        }

        [DllImport("winmm.dll")]
        private static extern uint timeBeginPeriod(uint milliseconds);

        [DllImport("winmm.dll")]
        private static extern uint timeEndPeriod(uint milliseconds);
    }
}
