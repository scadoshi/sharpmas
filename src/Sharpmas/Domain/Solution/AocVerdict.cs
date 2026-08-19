using System.Diagnostics;

namespace Sharpmas.Domain.Solution;

/// <summary>What adventofcode.com said about a submission.</summary>
/// <remarks>
/// Each part is graded exactly once, so a second correct answer comes back as
/// <see cref="AlreadySolved"/> rather than another confirmation. A record
/// hierarchy rather than an enum because <see cref="Cooldown"/> carries text.
/// </remarks>
public abstract record AocVerdict
{
    /// <summary>Graded and accepted. A new star.</summary>
    public sealed record Correct : AocVerdict;

    /// <summary>Graded and rejected, with no direction given.</summary>
    public sealed record Incorrect : AocVerdict;

    /// <summary>Rejected, and the site said the answer was too low.</summary>
    public sealed record Low : AocVerdict;

    /// <summary>Rejected, and the site said the answer was too high.</summary>
    public sealed record High : AocVerdict;

    /// <summary>Refused to grade: an answer went in too recently.</summary>
    public sealed record Cooldown(string Wait) : AocVerdict;

    /// <summary>The part is already solved, so nothing was graded.</summary>
    public sealed record AlreadySolved : AocVerdict;

    /// <summary>How the verdict reads in a part's output line.</summary>
    /// <remarks>
    /// Sealed so the cases cannot generate their own, which would silently
    /// replace this one.
    /// </remarks>
    public sealed override string ToString()
    {
        return this switch
        {
            Correct => "correct",
            Incorrect => "incorrect",
            Low => "low",
            High => "high",
            Cooldown(string wait) => $"rate limited, {wait} left to wait",
            AlreadySolved => "already solved",
            _ => throw new UnreachableException(
                $"unhandled {nameof(AocVerdict)}: {GetType().Name}"
            ),
        };
    }
}
