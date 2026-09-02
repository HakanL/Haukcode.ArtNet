using System.Globalization;
using Haukcode.ArtNet;

namespace Haukcode.ArtNet.TimeCodeGenerator;

public enum ScriptAction
{
    Play,
    Pause,
    Hold,
    Wait,
    Stop,
    Seek
}

public readonly record struct ScriptStep(ScriptAction Action, double DurationSeconds, TimeCode? SeekTarget)
{
    /// <summary>No duration: play/pause/hold run until quit or <c>--duration</c>.</summary>
    public bool Unbounded => DurationSeconds < 0;
}

/// <summary>
/// Parses a semicolon-separated transport script so the generator can be driven without a
/// keyboard: <c>play 14s; hold 3s; seek 01:00:08:00; play 6s</c>.
/// </summary>
public static class GeneratorScript
{
    public static IReadOnlyList<ScriptStep> Parse(string script, ArtTimeCodeTypes type)
    {
        var steps = new List<ScriptStep>();
        foreach (string raw in script.Split(';', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
            steps.Add(ParseStep(raw, type));

        if (steps.Count == 0)
            throw new OptionException("--script is empty");

        for (int i = 0; i < steps.Count - 1; i++)
        {
            if (steps[i].Unbounded)
                throw new OptionException($"--script step {i + 1} ('{steps[i].Action.ToString().ToLowerInvariant()}') has no duration, so later steps would never run");
        }

        return steps;
    }

    public static void Apply(ArtTimeCodeGenerator generator, ScriptStep step)
    {
        switch (step.Action)
        {
            case ScriptAction.Play:
                generator.Hold = false;
                generator.Paused = false;
                break;
            case ScriptAction.Pause:
            case ScriptAction.Wait:
                generator.Hold = false;
                generator.Paused = true;
                break;
            case ScriptAction.Hold:
                generator.Paused = false;
                generator.Hold = true;
                break;
            case ScriptAction.Stop:
                generator.Hold = false;
                generator.Paused = true;
                generator.Reset();
                break;
            case ScriptAction.Seek:
                generator.Seek(step.SeekTarget!.Value);
                break;
        }
    }

    private static ScriptStep ParseStep(string raw, ArtTimeCodeTypes type)
    {
        string[] parts = raw.Split((char[]?)null, 2, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        string command = parts[0].ToLowerInvariant();
        string? rest = parts.Length > 1 ? parts[1] : null;

        return command switch
        {
            "play" => new ScriptStep(ScriptAction.Play, ParseOptionalDuration(command, rest), null),
            "pause" => new ScriptStep(ScriptAction.Pause, ParseOptionalDuration(command, rest), null),
            "hold" => new ScriptStep(ScriptAction.Hold, ParseOptionalDuration(command, rest), null),
            "wait" => new ScriptStep(ScriptAction.Wait, ParseRequiredDuration(command, rest), null),
            "stop" => rest == null
                ? new ScriptStep(ScriptAction.Stop, 0, null)
                : throw new OptionException("stop takes no argument"),
            "seek" => ParseSeek(rest, type),
            _ => throw new OptionException($"Unknown --script command '{parts[0]}' (play, pause, hold, wait, stop, seek)")
        };
    }

    private static ScriptStep ParseSeek(string? rest, ArtTimeCodeTypes type)
    {
        if (string.IsNullOrWhiteSpace(rest))
            throw new OptionException("seek needs a timecode HH:MM:SS:FF");

        if (!TimeCode.TryParse(rest, type, out var target, out string? error))
            throw new OptionException($"seek: {error}");

        return new ScriptStep(ScriptAction.Seek, 0, target);
    }

    private static double ParseRequiredDuration(string command, string? rest)
    {
        if (string.IsNullOrWhiteSpace(rest))
            throw new OptionException($"{command} needs a duration (e.g. 2s or 500ms)");

        return ParseDuration(command, rest);
    }

    private static double ParseOptionalDuration(string command, string? rest) =>
        string.IsNullOrWhiteSpace(rest) ? -1 : ParseDuration(command, rest);

    private static double ParseDuration(string command, string text)
    {
        text = text.Trim();
        double scale = 1;
        if (text.EndsWith("ms", StringComparison.OrdinalIgnoreCase))
        {
            scale = 0.001;
            text = text[..^2];
        }
        else if (text.EndsWith("s", StringComparison.OrdinalIgnoreCase))
        {
            text = text[..^1];
        }

        if (!double.TryParse(text, CultureInfo.InvariantCulture, out double value) || value < 0)
            throw new OptionException($"{command}: '{text}' is not a duration");

        return value * scale;
    }
}

/// <summary>Walks a parsed script on the generator's wall clock.</summary>
public sealed class ScriptRunner
{
    private readonly IReadOnlyList<ScriptStep> steps;
    private int index;
    private double stepStartedAt;
    private bool finished;

    public ScriptRunner(IReadOnlyList<ScriptStep> steps)
    {
        this.steps = steps;
    }

    public bool Finished => this.finished;

    public ScriptStep Current => this.steps[Math.Min(this.index, this.steps.Count - 1)];

    public void Start(ArtTimeCodeGenerator generator) => BeginStep(generator, elapsedSeconds: 0);

    /// <returns><see langword="true"/> when the script has no more steps.</returns>
    public bool Tick(ArtTimeCodeGenerator generator, double elapsedSeconds)
    {
        if (this.finished)
            return true;

        var step = this.steps[this.index];
        if (step.Unbounded || elapsedSeconds - this.stepStartedAt < step.DurationSeconds)
            return false;

        this.index++;
        BeginStep(generator, elapsedSeconds);

        return this.finished;
    }

    private void BeginStep(ArtTimeCodeGenerator generator, double elapsedSeconds)
    {
        while (this.index < this.steps.Count)
        {
            GeneratorScript.Apply(generator, this.steps[this.index]);

            if (this.steps[this.index].DurationSeconds > 0 || this.steps[this.index].Unbounded)
            {
                this.stepStartedAt = elapsedSeconds;
                return;
            }

            this.index++;
        }

        this.finished = true;
    }
}
