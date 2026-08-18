namespace Sharpmas.Domain.Solution.Year2016.Day01;

/// <summary>A day, ready to be filled in. Copy the folder, rename the namespace.</summary>
/// <remarks>
/// Compiled on every build but never registered, so copying it starts from
/// something that cannot have drifted from <see cref="ISolution{TSelf}"/>. A
/// template that only lives in a README goes stale without anyone noticing.
///
/// See the repo README for the two steps: write the day, then add a line to
/// <c>Solvers</c> in <c>Inbound/Solve/SolveRun.cs</c>.
/// </remarks>
public class Puzzle : ISolution<Puzzle>
{
    /// <summary>
    /// Parsed state. Hold whatever both parts read, not the raw text, once a day
    /// needs more than a string.
    /// </summary>
    public required string Input { get; init; }

    /// <summary>Parses once, so both parts are reads over the result.</summary>
    /// <remarks>
    /// Throw when the input will not parse. That ends this day and leaves the
    /// others alone, which is why it is not caught anywhere below.
    /// </remarks>
    public static Puzzle Parse(string input) => new() { Input = input.Trim() };

    /// <summary>
    /// <see cref="Answer.Solved"/> for something submittable,
    /// <see cref="Answer.Visual"/> for art you read yourself,
    /// <see cref="Answer.None"/> when there is genuinely no answer.
    /// </summary>
    /// <remarks>
    /// Throwing means the day is broken, which is not the same as having no
    /// answer. It stops this part only; the other still runs.
    /// </remarks>
    public Answer PartOne() => new Answer.None();

    /// <summary>Same contract as <see cref="PartOne"/>. Day 25 has no second puzzle.</summary>
    public Answer PartTwo() => new Answer.None();
}
