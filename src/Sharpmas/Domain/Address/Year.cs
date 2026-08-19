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
        ArgumentOutOfRangeException.ThrowIfGreaterThan(year, Latest());
        ArgumentOutOfRangeException.ThrowIfLessThan(year, FirstYear);
        Value = year;
    }

    /// <summary>Days this event published: 25, except 2025 ran 12.</summary>
    public int DaysIn()
    {
        if (Value == 2025)
        {
            return 12;
        }
        return 25;
    }

    /// <summary>The latest event that has actually been published.</summary>
    /// <remarks>
    /// A new event drops each December, so for most of the year the current
    /// calendar year has nothing in it yet and the answer is the year before.
    /// </remarks>
    public static int Latest()
    {
        var now = DateTime.Now;
        if (now.Month == 12)
        {
            return now.Year;
        }
        else
        {
            return now.Year - 1;
        }
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}
