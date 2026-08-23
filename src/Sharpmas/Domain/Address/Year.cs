namespace Sharpmas.Domain.Address;

/// <summary>A validated Advent of Code event year.</summary>
/// <remarks>
/// The constructor is the only way in, so holding a <see cref="Year"/> means it
/// names an event that was actually published.
/// </remarks>
public sealed class Year
{
    /// <summary>The first Advent of Code.</summary>
    public const int FirstYear = 2015;

    /// <summary>The actual year value.</summary>
    public int Value { get; }

    /// <summary>Creates a validated year.</summary>
    public Year(int year)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(year, Latest(), nameof(year));
        ArgumentOutOfRangeException.ThrowIfLessThan(year, FirstYear, nameof(year));
        Value = year;
    }

    /// <summary>Days this event published: 25, except 2025 ran 12.</summary>
    public int DaysIn() =>
        Value switch
        {
            2025 => 12,
            _ => 25,
        };

    /// <summary>The latest event that has actually been published.</summary>
    /// <remarks>
    /// A new event drops each December, so for most of the year the current
    /// calendar year has nothing in it yet and the answer is the year before.
    /// </remarks>
    public static int Latest() =>
        DateTime.Now.Month switch
        {
            12 => DateTime.Now.Year,
            _ => DateTime.Now.Year - 1,
        };

    public override string ToString() => Value.ToString();
}
