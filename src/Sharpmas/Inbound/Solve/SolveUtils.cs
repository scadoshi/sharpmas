using Sharpmas.Domain.Address;
using Sharpmas.Domain.Solution;
using Sharpmas.Outbound.Client;

namespace Sharpmas.Inbound.Solve;

/// <summary>Submitting, and asking before a submit run that would post everything.</summary>
public static class SolveUtils
{
    /// <summary>Submits the answer if the solver backed it, with AOC's reply attached.</summary>
    /// <remarks>
    /// A wrong answer costs an escalating cooldown, so the solver verdict gates
    /// the send. An unsupported puzzle goes through anyway, since the solver
    /// cannot judge it either way and that is the live-event case where being
    /// ahead of it is exactly when submitting matters.
    /// </remarks>
    public static async Task<Outcome> Submit(
        AocClient aoc,
        Day day,
        Part part,
        Outcome outcome
    )
    {
        if (outcome.GetValue() is not string value)
        {
            return outcome;
        }
        if (outcome.SolverVerdict is not (SolverVerdict.Correct or SolverVerdict.Unsupported))
        {
            return outcome;
        }

        return outcome.WithAocVerdict(await aoc.SubmitAnswer(day, part, value));
    }

    /// <summary>Asks before an unfiltered submit run, which would post every solved day.</summary>
    /// <remarks>
    /// Goes to stderr so redirecting output cannot swallow it. Closed stdin
    /// reads as no, so a piped run never submits by accident.
    /// </remarks>
    public static bool Confirm(int count)
    {
        Console.Error.WriteLine(
            $"About to submit up to {count} answers to adventofcode.com, across every "
                + "year and day. Wrong answers are rate limited. Narrow it with --year or "
                + "--day, or pass --yes to skip this."
        );
        Console.Error.Write("Continue? [y/N] ");

        var reply = Console.In.ReadLine();
        return reply?.Trim() is "y" or "Y" or "yes" or "Yes";
    }
}
