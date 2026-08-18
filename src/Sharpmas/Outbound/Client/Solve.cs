using System.Diagnostics;
using Sharpmas.Domain.Address;
using Sharpmas.Domain.Solution;

namespace Sharpmas.Outbound.Client;

/// <summary>Runs a day and gathers what came of it.</summary>
/// <remarks>
/// Sits beside the clients rather than in the domain because validating holds
/// one, which is the dependency the domain is not allowed to have.
/// </remarks>
public static class Solver
{
    /// <summary>Runs both parts, checking each answer against the solver when asked.</summary>
    /// <remarks>
    /// Validation runs after both parts are measured, so no timing includes a
    /// network round trip, and only a submittable answer is checked at all.
    ///
    /// A failing part goes into its own outcome rather than being propagated, so
    /// the other part still runs. Only a failure to parse ends the day, which is
    /// why that call sits outside the catch.
    ///
    /// Generic because a static interface member has nothing to dispatch on, so
    /// <c>T.Parse</c> is reachable only through a type parameter.
    /// </remarks>
    public static async Task<Solved> Solve<T>(
        SolverClient client,
        bool validate,
        string input,
        Day day
    )
        where T : ISolution<T>
    {
        var timer = Stopwatch.StartNew();
        var solution = T.Parse(input);
        var parse = timer.Elapsed;

        timer.Restart();
        var one = new Outcome(Attempt(solution.PartOne), timer.Elapsed);

        timer.Restart();
        var two = new Outcome(Attempt(solution.PartTwo), timer.Elapsed);

        if (validate)
        {
            one = await Validated(client, one, day, Part.One, input);
            two = await Validated(client, two, day, Part.Two, input);
        }

        return new Solved
        {
            Parse = parse,
            One = one,
            Two = two,
        };
    }

    /// <summary>Runs one part, keeping a failure rather than letting it escape.</summary>
    static AnswerResult Attempt(Func<Answer> part)
    {
        try
        {
            return new AnswerResult.Ok(part());
        }
        catch (Exception e)
        {
            return new AnswerResult.Err(e);
        }
    }

    /// <summary>Attaches a solver verdict, skipping anything unsubmittable.</summary>
    static async Task<Outcome> Validated(
        SolverClient client,
        Outcome outcome,
        Day day,
        Part part,
        string input
    )
    {
        if (outcome.GetValue() is not string value)
        {
            return outcome;
        }
        return outcome.WithVerdict(await client.ValidateAnswer(day, input, part, value));
    }
}
