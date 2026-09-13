using System.Text;
using Sharpmas.Domain.Address;
using Sharpmas.Extensions;

namespace Sharpmas.Domain.Solution;

/// <summary>A run's time, totalled across every day it solved.</summary>
/// <remarks>
/// Parsing counts once per day and solving once per part, so the two means have
/// different denominators and the printed lines say which is which.
/// </remarks>
public sealed class Totals
{
    TimeSpan parsedTotal = TimeSpan.Zero;
    TimeSpan solvedTotal = TimeSpan.Zero;
    int parts;
    Slowest? slowest;

    /// <summary>The part that took longest, and which one it was.</summary>
    sealed record Slowest(Day Day, Part Part, TimeSpan Elapsed);

    /// <summary>How many days have been folded in.</summary>
    public int Days { get; private set; }

    /// <summary>Null when no day ran, since there is nothing to divide by.</summary>
    public TimeSpan? ParseMean => Days > 0 ? parsedTotal / Days : null;

    /// <summary>Null when no part produced an answer, which a run of stubs manages.</summary>
    public TimeSpan? SolveMean => parts > 0 ? solvedTotal / parts : null;

    /// <summary>Folds in one day's run.</summary>
    /// <remarks>
    /// Only parts that produced an answer count towards solving; the day counts
    /// towards parsing either way.
    /// </remarks>
    public void Add(Day day, Solved solved)
    {
        parsedTotal += solved.ParsedIn;
        Days++;

        foreach (
            var (part, outcome) in new[]
            {
                (Part.One, solved.PartOne),
                (Part.Two, solved.PartTwo),
            }
        )
        {
            if (outcome.SolveTime is not TimeSpan elapsed)
            {
                continue;
            }
            solvedTotal += elapsed;
            parts++;
            if (slowest is null || elapsed > slowest.Elapsed)
            {
                slowest = new Slowest(day, part, elapsed);
            }
        }
    }

    /// <summary>A block of whole lines, each one ended.</summary>
    /// <remarks>
    /// Renders whatever it holds, including a run of nothing. When that is worth
    /// showing is the caller's to decide.
    /// </remarks>
    public override string ToString()
    {
        var message = new StringBuilder();
        message.AppendLine($"total time spent parsing: {parsedTotal.Formatted()}");
        if (ParseMean is TimeSpan parseMean)
        {
            message.AppendLine($"average parse time per day: {parseMean.Formatted()}");
        }
        message.AppendLine($"total time spent solving: {solvedTotal.Formatted()}");
        if (SolveMean is TimeSpan solveMean)
        {
            message.AppendLine($"average solve time per part: {solveMean.Formatted()}");
        }
        if (slowest is Slowest s)
        {
            message.AppendLine(
                $"slowest part: year {s.Day.Year} day {s.Day.Value} "
                    + $"part {s.Part.Word} [{s.Elapsed.Formatted()}]"
            );
        }
        return message.ToString();
    }
}
