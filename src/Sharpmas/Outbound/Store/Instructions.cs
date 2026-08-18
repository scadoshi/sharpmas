namespace Sharpmas.Outbound.Store;

/// <summary>A day's puzzle text, one part per property.</summary>
/// <remarks>
/// Part two stays null until part one is solved, since the site does not
/// publish it before then.
/// </remarks>
public sealed class Instructions
{
    public required string PartOne { get; init; }
    public required string? PartTwo { get; init; }
}
