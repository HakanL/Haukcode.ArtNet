using System.Globalization;
using System.Net;

namespace Haukcode.ArtNet.TimeCodeGenerator;

public sealed class OptionException(string message) : Exception(message);

public sealed class Options
{
    public ArtTimeCodeTypes Type { get; set; } = ArtTimeCodeTypes.Smpte;

    public string Start { get; set; } = "00:00:00:00";

    public byte StreamId { get; set; }

    public IPAddress? LocalAddress { get; set; }

    public IPAddress? Destination { get; set; }

    public int Port { get; set; } = ArtNetClient.DefaultPort;

    /// <summary>0 = run until stopped.</summary>
    public double DurationSeconds { get; set; }

    public double JitterMs { get; set; }

    public double DropPercent { get; set; }

    /// <summary>Start parked on the start value instead of playing immediately.</summary>
    public bool Stopped { get; set; }

    public bool ListInterfaces { get; set; }

    public bool Quiet { get; set; }

    public bool Help { get; set; }

    public const string Usage = """
        ArtNetTimeCodeGenerator - sends ArtTimeCode (OpTimeCode 0x9700) frames for testing receivers

        Usage: ArtNetTimeCodeGenerator [options]

          -f, --fps <rate>        24 | 25 | 29.97 | 30 (or film | ebu | df | smpte). Default 30
          -s, --start <timecode>  Start value HH:MM:SS:FF (';' before FF for drop-frame). Default 00:00:00:00
              --stream <id>       ArtTimeCode StreamId 0-255. Default 0
          -d, --destination <ip>  Unicast target. Default: subnet broadcast of the local interface
          -l, --local <ip>        Local interface to send from. Default: first Ethernet/Wi-Fi adapter
          -p, --port <port>       UDP port. Default 6454
          -t, --duration <sec>    Exit after this many seconds. Default: run until Q or Ctrl+C
              --stopped           Start parked on the start value; press Space/P to play
              --jitter <ms>       Randomize each frame's send moment by +/- ms (timing robustness test)
              --drop <percent>    Randomly skip this percentage of frames (packet loss test)
              --list-interfaces   Print the usable local interfaces and exit
          -q, --quiet             No live status line
          -h, --help              This text

        Transport keys while running:
          Space/P  play / pause (pause stops sending, clock frozen)
          S        stop: pause and rewind to the start value
          R        rewind to the start value (keeps playing if playing)
          L        locate: type a timecode HH:MM:SS:FF and continue from there
          C        clock: jump to the system time of day
          H        hold: keep sending the same frame (receiver sees a frozen clock)
          + / -    jump 10 s forward/back          . / ,  step one frame forward/back
          1-4      switch rate: 1=24 fps, 2=25 fps, 3=29.97 df, 4=30 fps
          Q/Esc    quit

        Examples:
          ArtNetTimeCodeGenerator --fps 25 --start 00:59:50:00
          ArtNetTimeCodeGenerator --fps 29.97 --start 01:00:00;02 --destination 192.168.240.196
          ArtNetTimeCodeGenerator --stream 3 --jitter 5 --drop 2 --duration 60
        """;

    public static Options Parse(string[] args)
    {
        var options = new Options();

        for (int i = 0; i < args.Length; i++)
        {
            string arg = args[i];
            string? inlineValue = null;

            int equals = arg.IndexOf('=');
            if (arg.StartsWith("--") && equals > 0)
            {
                inlineValue = arg[(equals + 1)..];
                arg = arg[..equals];
            }

            string Value()
            {
                if (inlineValue != null)
                    return inlineValue;

                if (i + 1 >= args.Length)
                    throw new OptionException($"Option {arg} needs a value");

                return args[++i];
            }

            switch (arg.ToLowerInvariant())
            {
                case "-f":
                case "--fps":
                case "--type":
                    options.Type = ParseType(Value());
                    break;

                case "-s":
                case "--start":
                    options.Start = Value();
                    break;

                case "--stream":
                    options.StreamId = ParseNumber<byte>(arg, Value());
                    break;

                case "-d":
                case "--destination":
                    options.Destination = ParseAddress(arg, Value());
                    break;

                case "-l":
                case "--local":
                    options.LocalAddress = ParseAddress(arg, Value());
                    break;

                case "-p":
                case "--port":
                    options.Port = ParseNumber<ushort>(arg, Value());
                    break;

                case "-t":
                case "--duration":
                    options.DurationSeconds = ParseNumber<double>(arg, Value());
                    break;

                case "--jitter":
                    options.JitterMs = ParseNumber<double>(arg, Value());
                    break;

                case "--drop":
                    options.DropPercent = ParseNumber<double>(arg, Value());
                    if (options.DropPercent is < 0 or > 100)
                        throw new OptionException("--drop must be between 0 and 100");
                    break;

                case "--stopped":
                case "--paused":
                    options.Stopped = true;
                    break;

                case "--list-interfaces":
                case "--list":
                    options.ListInterfaces = true;
                    break;

                case "-q":
                case "--quiet":
                    options.Quiet = true;
                    break;

                case "-h":
                case "--help":
                case "-?":
                case "/?":
                    options.Help = true;
                    break;

                default:
                    throw new OptionException($"Unknown option '{arg}'");
            }
        }

        return options;
    }

    private static ArtTimeCodeTypes ParseType(string value) => value.Trim().ToLowerInvariant() switch
    {
        "24" or "film" => ArtTimeCodeTypes.Film,
        "25" or "ebu" => ArtTimeCodeTypes.Ebu,
        "29.97" or "29.97df" or "29,97" or "df" or "drop" or "dropframe" => ArtTimeCodeTypes.DropFrame,
        "30" or "smpte" or "ndf" => ArtTimeCodeTypes.Smpte,
        _ => throw new OptionException($"Unknown frame rate '{value}', expected 24, 25, 29.97 or 30")
    };

    private static IPAddress ParseAddress(string option, string value)
    {
        if (!IPAddress.TryParse(value, out var address) || address.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork)
            throw new OptionException($"{option}: '{value}' is not an IPv4 address");

        return address;
    }

    private static T ParseNumber<T>(string option, string value) where T : IParsable<T>
    {
        if (!T.TryParse(value, CultureInfo.InvariantCulture, out var result))
            throw new OptionException($"{option}: '{value}' is not a valid {typeof(T).Name.ToLowerInvariant()} value");

        return result;
    }
}
