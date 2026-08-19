using System.Diagnostics;
using Sharpmas.Extensions;

namespace Sharpmas.Domain.Solution;

/// <summary>One part's answer and everything learned about it afterwards.</summary>
/// <remarks>
/// The four properties have three different sources: <see cref="Answer"/> is
/// computed, <see cref="Elapsed"/> is measured, and the two verdicts arrive over
/// the network. Only a submittable answer can carry a verdict, which the
/// attaching methods enforce.
/// </remarks>
public record Outcome
{
    /// <summary>What the part produced.</summary>
    public AnswerResult AnswerResult { get; }

    /// <summary>Time computing the answer, never the network.</summary>
    public TimeSpan Elapsed { get; }

    /// <summary>The solver's verdict, or null when nothing checked it.</summary>
    /// <remarks>
    /// Repeatable, so it is what gates submission. Set only from inside, so
    /// <see cref="WithVerdict"/> is the one way it can be attached. A public
    /// <c>init</c> would let a caller reach it through a <c>with</c> expression
    /// and skip the submittable-answer check.
    /// </remarks>
    public SolverVerdict? Verdict { get; private init; }

    /// <summary>AOC's reply, or null when nothing was submitted.</summary>
    /// <remarks>
    /// Says whether the star exists. Set only from inside, for the same reason
    /// as <see cref="Verdict"/>.
    /// </remarks>
    public AocVerdict? Submission { get; private init; }

    /// <summary>An outcome with nothing known about it yet.</summary>
    public Outcome(AnswerResult answerResult, TimeSpan elapsed)
    {
        AnswerResult = answerResult;
        Elapsed = elapsed;
        Verdict = null;
        Submission = null;
    }

    /// <summary>The submittable text, or null when there is none.</summary>
    /// <remarks>
    /// A failed part has nothing to submit, and neither does art or an absent
    /// answer, so this is the one check the attaching methods need.
    /// </remarks>
    public string? GetValue()
    {
        return AnswerResult switch
        {
            AnswerResult.Ok(Answer.Value(string value)) => value,
            AnswerResult.Ok(_) or AnswerResult.Err(_) => null,
            _ => throw new UnreachableException(
                $"unhandled {nameof(AnswerResult)}: {GetType().Name}"
            ),
        };
    }

    /// <summary>Attaches a solver verdict if there is something to check.</summary>
    /// <remarks>
    /// Non-destructive, like every other <c>With</c> in .NET: the receiver is
    /// left alone and a copy carrying the verdict comes back. An unsubmittable
    /// answer has nothing to check, so it returns itself unchanged.
    /// </remarks>
    public Outcome WithVerdict(SolverVerdict verdict)
    {
        if (GetValue() is null)
        {
            return this;
        }
        return this with { Verdict = verdict };
    }

    /// <summary>Attaches AOC's reply if there was something to submit.</summary>
    /// <remarks>Same shape as <see cref="WithVerdict"/>.</remarks>
    public Outcome WithSubmission(AocVerdict submission)
    {
        if (GetValue() is null)
        {
            return this;
        }
        return this with { Submission = submission };
    }

    /// <summary>The answer, then notes, then timing.</summary>
    /// <remarks>
    /// One line, except for art, which brings its own. AOC's word supersedes
    /// the solver's, so a graded part reads as starred rather than repeating
    /// that the solver agreed. Any ungraded reply shows beside the solver's.
    /// </remarks>
    public override string ToString()
    {
        string message = "";
        message += AnswerResult switch
        {
            AnswerResult.Ok(Answer answer) => answer.ToString(),
            // The whole chain, since the outermost message rarely names the day.
            AnswerResult.Err(Exception exception) =>
                $"error: {string.Join(
                ": ",
                exception.Causes().Select(cause => cause.Message)
            )}",
            _ => throw new UnreachableException(
                $"unhandled {nameof(AnswerResult)}: {AnswerResult.GetType().Name}"
            ),
        };
        string notes = (Verdict, Submission) switch
        {
            (_, AocVerdict.Correct) => "new star",
            (_, AocVerdict.AlreadySolved) => "starred",
            (SolverVerdict v, AocVerdict s) => $"{v}, {s}",
            (SolverVerdict v, null) => $"{v}",
            (null, AocVerdict s) => $"{s}",
            (null, null) => "",
        };
        if (notes.Length != 0)
        {
            message += $" ({notes})";
        }
        // Art ends its own line, so the timing needs no space in front of it.
        if (!message.EndsWith('\n'))
        {
            message += " ";
        }
        message += $"[{Elapsed.Formatted()}]";
        return message;
    }
}
