namespace Sharpmas.Domain.Solution;

public abstract record AnswerResult
{
    public sealed record Ok(Answer Answer) : AnswerResult;

    public sealed record Err(Exception Exception) : AnswerResult;
}
