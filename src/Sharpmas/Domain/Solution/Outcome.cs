namespace Sharpmas.Domain.Solution;

/// <summary>
/// One part's answer and everything learned about it afterwards.
/// </summary>
/// <remarks>
/// The four properties have three different sources: <see cref="Answer"/> is
/// computed, <see cref="Elapsed"/> is measured, and the two verdicts arrive over
/// the network. Only a submittable answer can carry a verdict, which the
/// attaching methods enforce.
/// </remarks>
public record Outcome
{
    /// <summary>What the part produced.</summary>
    public Answer Answer { get; }

    /// <summary>
    /// Time to compute the answer. Never includes a network round trip.
    /// </summary>
    public TimeSpan Elapsed { get; }

    /// <summary>
    /// From the third-party solver, or null when nothing checked it.
    /// </summary>
    /// <remarks>
    /// Repeatable, so it is what gates submission. Set only from inside, so
    /// <see cref="WithVerdict"/> is the one way it can be attached. A public
    /// <c>init</c> would let a caller reach it through a <c>with</c> expression
    /// and skip the submittable-answer check.
    /// </remarks>
    public SolverVerdict? Verdict { get; private init; }

    /// <summary>
    /// From adventofcode.com, or null when nothing was submitted.
    /// </summary>
    /// <remarks>
    /// Says whether the star exists. Set only from inside, for the same reason
    /// as <see cref="Verdict"/>.
    /// </remarks>
    public AocVerdict? Submission { get; private init; }

    /// <summary>
    /// An outcome with nothing known about it yet.
    /// </summary>
    /// <param name="answer">What the part produced.</param>
    /// <param name="elapsed">How long producing it took.</param>
    public Outcome(Answer answer, TimeSpan elapsed)
    {
        Answer = answer;
        Elapsed = elapsed;
        Verdict = null;
        Submission = null;
    }

    /// <summary>
    /// Attaches a solver verdict, ignored unless there is something to check.
    /// </summary>
    /// <remarks>
    /// Non-destructive, like every other <c>With</c> in .NET: the receiver is
    /// left alone and a copy carrying the verdict comes back. An unsubmittable
    /// answer has nothing to check, so it returns itself unchanged.
    /// </remarks>
    /// <param name="verdict">What the solver made of the answer.</param>
    /// <returns>A copy carrying <paramref name="verdict"/>.</returns>
    public Outcome WithVerdict(SolverVerdict verdict)
    {
        if (Answer.GetValue() is null)
        {
            return this;
        }
        return this with { Verdict = verdict };
    }
}
