namespace Sharpmas.Inbound.Solve;

/// <summary>What a solve run was asked to do.</summary>
public sealed class SolveArgs
{
    /// <summary>Year to run, or null for all.</summary>
    public int? Year { get; init; }

    /// <summary>Day to run, or null for all.</summary>
    public int? Day { get; init; }

    /// <summary>Check answers against the third-party solver, one request per part.</summary>
    public bool Validate { get; init; }

    /// <summary>Submit answers to adventofcode.com, one request per part.</summary>
    /// <remarks>
    /// Validates first and only sends what the solver agrees with, because a
    /// wrong answer costs an escalating cooldown and the check is free.
    /// </remarks>
    public bool Submit { get; init; }

    /// <summary>Skip the confirmation prompt when submitting unfiltered.</summary>
    /// <remarks>No short flag: this one is worth typing out.</remarks>
    public bool Yes { get; init; }

    /// <summary>True when a submit run would post every solved day, not just one.</summary>
    public bool SubmittingEverything => Submit && Year is null && Day is null;
}
