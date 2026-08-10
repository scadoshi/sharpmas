namespace Sharpmas.Domain.Solutions;

public abstract record Answer
{
    public record Value(string Data) : Answer;

    public record Visual(string Data) : Answer;

    public record None : Answer;

    public static Answer Solved(string data)
    {
        return new Value(data);
    }
}
