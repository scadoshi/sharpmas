namespace Sharpmas.Domain.Solution;

/// <summary>What a part produced, or why it could not.</summary>
/// <remarks>
/// Stands in for Rust's <c>Result</c>, which C# has no equivalent of. A failure
/// is held here rather than thrown out of the run, so one broken part does not
/// hide the other's answer. Two nullable fields would allow both set and both
/// null, neither of which means anything.
/// </remarks>
public abstract record AnswerResult
{
    /// <summary>The part ran and produced an answer.</summary>
    public sealed record Ok(Answer Answer) : AnswerResult;

    /// <summary>The part threw, and this is what it threw.</summary>
    public sealed record Err(Exception Exception) : AnswerResult;
}
