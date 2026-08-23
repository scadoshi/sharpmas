namespace Sharpmas.Domain.Address;

/// <summary>A validated narrowing of the published days. Null means all of them.</summary>
/// <remarks>
/// Validation happens here, eagerly, so expanding a filter cannot fail and an
/// impossible one errors up front rather than sweeping the range and matching
/// nothing in silence.
/// </remarks>
public sealed class Filter
{
    public Year? Year { get; }

    /// <summary>An int rather than a <see cref="Address.Day"/>, since a day filter
    /// with no year is not an address: day 13 is valid in 2015 and not in 2025.</summary>
    public int? Day { get; }

    /// <summary>Validates whatever was given: each side alone, and the pair strictly.</summary>
    public Filter(int? year, int? day)
    {
        switch (year, day)
        {
            case (int y, int d):
            {
                Year = new Year(y);
                _ = new Day(Year, d);
                Day = d;
                break;
            }
            case (int y, null):
            {
                Year = new Year(y);
                break;
            }
            case (null, int d):
            {
                ArgumentOutOfRangeException.ThrowIfLessThan(d, 1, nameof(d));
                ArgumentOutOfRangeException.ThrowIfGreaterThan(d, Address.Day.FinalDay, nameof(d));
                Day = d;
                break;
            }
            case (null, null):
            {
                break;
            }
        }
    }
}
