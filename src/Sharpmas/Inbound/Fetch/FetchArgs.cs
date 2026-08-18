namespace Sharpmas.Inbound.Fetch;

/// <summary>What a fetch run was asked to download.</summary>
/// <remarks>
/// Year and day are filters rather than a lookup, so omitting one means all of
/// them and no combination of flags needs special handling.
/// </remarks>
public sealed class FetchArgs
{
    /// <summary>Year to fetch inputs for, or null for all.</summary>
    public int? Year { get; init; }

    /// <summary>Day to fetch inputs for, or null for all.</summary>
    public int? Day { get; init; }
}
