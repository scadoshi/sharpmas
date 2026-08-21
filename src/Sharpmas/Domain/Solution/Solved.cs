namespace Sharpmas.Domain.Solution;

public class Solved
{
    public required TimeSpan ParsedIn { get; init; }
    public required Outcome PartOne { get; init; }
    public required Outcome PartTwo { get; init; }

    public TimeSpan TotalElapsed => ParsedIn + PartOne.Elapsed + PartTwo.Elapsed;
}
