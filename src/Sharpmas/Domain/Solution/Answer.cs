using System.Diagnostics;

namespace Sharpmas.Domain.Solution;

/// <summary>
/// What one part of a puzzle produced. Nothing else.
/// </summary>
/// <remarks>
/// Verdicts and timings live on <see cref="Outcome"/>, since they come from
/// elsewhere: the solver, adventofcode.com, and a clock.
/// </remarks>
public abstract record Answer
{
    /// <summary>A submittable answer.</summary>
    public sealed record Value(string Data) : Answer;

    /// <summary>
    /// Art you read rather than submit.
    /// </summary>
    /// <remarks>
    /// Returned instead of printed, so solving does no IO.
    /// </remarks>
    public sealed record Visual(string Art) : Answer;

    /// <summary>
    /// Nothing to produce. Day 25 part two is the usual case.
    /// </summary>
    public sealed record None : Answer;

    /// <summary>
    /// A submittable answer. What a day returns.
    /// </summary>
    /// <param name="data">The answer as the site expects to receive it.</param>
    /// <returns>A <see cref="Value"/> holding <paramref name="data"/>.</returns>
    public static Answer Solved(string data)
    {
        return new Value(data);
    }

    /// <summary>
    /// The submittable text, if there is any.
    /// </summary>
    /// <remarks>
    /// The one question the rest of the tool asks an answer. Returning null for
    /// the other cases is what stops art or an absent answer being validated or
    /// submitted.
    /// </remarks>
    /// <returns>The text of a <see cref="Value"/>, otherwise null.</returns>
    public string? GetValue()
    {
        return this switch
        {
            Value(string data) => data,
            _ => null,
        };
    }

    /// <summary>
    /// How the answer reads in a part's output line.
    /// </summary>
    /// <remarks>
    /// Sealed so the cases cannot generate their own, which would silently
    /// replace this one.
    /// </remarks>
    /// <returns>
    /// The value or the art as-is, or <c>(none)</c> when there is nothing.
    /// </returns>
    /// <exception cref="UnreachableException">
    /// Thrown when a case is added without a line here.
    /// </exception>
    public sealed override string ToString()
    {
        return this switch
        {
            Value(string data) => $"{data}",
            Visual(string art) => $"{art}",
            None => "(none)",
            _ => throw new UnreachableException($"unhandled {nameof(Answer)}: {GetType().Name}"),
        };
    }
}
