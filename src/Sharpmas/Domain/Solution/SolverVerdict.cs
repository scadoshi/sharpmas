using System.Diagnostics;

namespace Sharpmas.Domain.Solution;

/// <summary>
/// What the third-party solver made of an answer.
/// </summary>
/// <remarks>
/// Repeatable, since the solver has no accounts and no memory, which is what
/// makes it usable as a gate before submitting. A record hierarchy rather than
/// an enum so it can carry its own rendering, matching <see cref="AocVerdict"/>.
/// </remarks>
public abstract record SolverVerdict
{
    /// <summary>The solver agrees.</summary>
    public sealed record Correct : SolverVerdict;

    /// <summary>The solver disagrees, with no direction to report.</summary>
    public sealed record Incorrect : SolverVerdict;

    /// <summary>Our answer is below the solver's.</summary>
    public sealed record Low : SolverVerdict;

    /// <summary>Our answer is above the solver's.</summary>
    public sealed record High : SolverVerdict;

    /// <summary>No implementation for that puzzle.</summary>
    public sealed record Unsupported : SolverVerdict;

    /// <summary>
    /// A verdict from a plain match or mismatch, where there is no direction to
    /// report.
    /// </summary>
    /// <param name="isCorrect">Whether the answers matched.</param>
    /// <returns><see cref="Correct"/> or <see cref="Incorrect"/>.</returns>
    public static SolverVerdict From(bool isCorrect)
    {
        return isCorrect ? new Correct() : new Incorrect();
    }

    /// <summary>
    /// A verdict from comparing our answer against the solver's.
    /// </summary>
    /// <remarks>
    /// Read from our side, so a negative comparison means ours was the low one.
    /// Normalised through <see cref="Math.Sign(int)"/> because a comparison only
    /// promises a sign, never a particular value.
    /// </remarks>
    /// <param name="comparison">Our answer compared against the solver's.</param>
    /// <returns><see cref="Low"/>, <see cref="Correct"/>, or <see cref="High"/>.</returns>
    public static SolverVerdict From(int comparison)
    {
        return Math.Sign(comparison) switch
        {
            -1 => new Low(),
            0 => new Correct(),
            1 => new High(),
            _ => throw new UnreachableException($"Math.Sign returned {comparison}"),
        };
    }

    /// <summary>
    /// How the verdict reads in a part's output line.
    /// </summary>
    /// <remarks>
    /// Sealed so the cases cannot generate their own, which would silently
    /// replace this one.
    /// </remarks>
    /// <returns>The verdict as a lowercase word.</returns>
    /// <exception cref="UnreachableException">
    /// Thrown when a case is added without a line here.
    /// </exception>
    public sealed override string ToString()
    {
        return this switch
        {
            Correct => "correct",
            Incorrect => "incorrect",
            Low => "low",
            High => "high",
            Unsupported => "unsupported",
            _ => throw new UnreachableException(
                $"unhandled {nameof(SolverVerdict)}: {GetType().Name}"
            ),
        };
    }
}
