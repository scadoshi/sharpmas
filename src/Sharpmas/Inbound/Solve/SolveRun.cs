using Sharpmas.Domain.Address;
using Sharpmas.Extensions;
using Sharpmas.Domain.Solution;
using Sharpmas.Outbound.Client;

namespace Sharpmas.Inbound.Solve;

/// <summary>Runs solutions, optionally validating and submitting them.</summary>
public static class SolveRun
{
    /// <summary>A day's solver, once its concrete type is known.</summary>
    delegate Task<Solved> Solver(SolverClient client, bool validate, string input, Day day);

    /// <summary>Every day that has been written. One line each.</summary>
    /// <remarks>
    /// Holding a delegate rather than calling means the registry can be asked
    /// whether a day exists without holding its input, which is what lets a run
    /// skip unwritten days before downloading and lets a submit run count first.
    ///
    /// Days are named fully qualified because every one of them is a
    /// <c>Puzzle</c> in its own namespace.
    /// </remarks>
    static readonly Dictionary<(int Year, int Day), Solver> Solvers = new()
    {
    };

    /// <summary>The solver for a day, or null when nobody has written one.</summary>
    static Solver? SolverFor(Day day) =>
        Solvers.TryGetValue((day.Year.Value, day.Value), out var solver) ? solver : null;

    /// <summary>How many parts a run over these filters would touch.</summary>
    static int PartCount(int? year, int? day) =>
        Day.Each(year, day).Count(d => SolverFor(d) is not null) * 2;

    /// <summary>Solves every day the filters allow.</summary>
    public static async Task Run(SolveArgs args)
    {
        // Submitting gates on a solver verdict, so it validates too.
        var validate = args.Validate || args.Submit;
        var solver = new SolverClient();

        var count = PartCount(args.Year, args.Day);
        if (args.SubmittingEverything && !args.Yes && count > 0 && !SolveUtils.Confirm(count))
        {
            Console.Error.WriteLine("Nothing submitted.");
            return;
        }

        // Built up front when submitting, so a bad cookie fails before any
        // solving. Otherwise built on first download, leaving cached runs
        // offline.
        var client = new LazyAocClient();
        if (args.Submit)
        {
            client.Connected();
        }

        foreach (var day in Day.Each(args.Year, args.Day))
        {
            // Asked before fetching, so a run over every year downloads nothing
            // for days it cannot solve.
            var run = SolverFor(day);
            if (run is null)
            {
                if (args.Day is not null)
                {
                    Console.Error.WriteLine($"year {day.Year} day {day.Value} has no solution yet");
                }
                continue;
            }

            var entry = await Inputs.EnsureEntry(client, day);

            Solved solved;
            try
            {
                solved = await run(solver, validate, entry.Input.Data, day);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"year {day.Year} day {day.Value} failed: {e.Message}");
                continue;
            }

            // Submit before printing, so each part reports what both checkers
            // said on one line.
            if (args.Submit)
            {
                var aoc = client.Connected();
                solved = new Solved
                {
                    Parse = solved.Parse,
                    One = await SolveUtils.Submit(aoc, day, Part.One, solved.One),
                    Two = await SolveUtils.Submit(aoc, day, Part.Two, solved.Two),
                };
            }

            // A new star on part one unlocks part two, which was still locked
            // when this run read the cache.
            if (solved.One.Submission is AocVerdict.Correct)
            {
                await Inputs.EnsureEntry(client, day);
            }

            Console.WriteLine(
                $"year {day.Year} day {day.Value} in {solved.Total.Formatted()} "
                    + $"({solved.Parse.Formatted()} parsing)"
            );
            Console.WriteLine($"  part one: {solved.One}");
            Console.WriteLine($"  part two: {solved.Two}");
        }
    }
}
