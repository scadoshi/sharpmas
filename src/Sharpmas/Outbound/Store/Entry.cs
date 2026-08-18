namespace Sharpmas.Outbound.Store;

/// <summary>Everything cached for one day.</summary>
public sealed class Entry
{
    public required Input Input { get; init; }
    public required Instructions Instructions { get; init; }
}
