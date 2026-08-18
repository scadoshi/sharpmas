namespace Sharpmas.Domain.Solution;

public class Solved
{
    public required TimeSpan Parse { get; init; }
    public required Outcome One { get; init; }
    public required Outcome Two { get; init; }

    public TimeSpan Total => Parse + One.Elapsed + Two.Elapsed;
}
