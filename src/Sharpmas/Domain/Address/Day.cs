namespace Sharpmas.Domain.Address;

/// <summary>A validated puzzle day within a validated year.</summary>
/// <remarks>
/// A day always carries its year, so it names one puzzle unambiguously. The
/// constructor takes a built <see cref="Address.Year"/> rather than a number, so a
/// day cannot exist without a year having been validated first.
/// </remarks>
public sealed class Day
{
    /// <summary>Christmas Day, whose second star is awarded rather than puzzled.</summary>
    public const int FinalDay = 25;

    /// <summary>The day of the event, counting from 1.</summary>
    public int Value { get; }

    /// <summary>The event this day belongs to.</summary>
    public Year Year { get; }

    /// <summary>Whether this day's second star is a puzzle rather than a reward.</summary>
    /// <remarks>
    /// Day 25's is given for holding every other star. The site shows it a
    /// closing note rather than a puzzle, so the tool never fetches one.
    /// </remarks>
    public bool HasSecondPuzzle => Value != FinalDay;

    /// <summary>Creates a validated day.</summary>
    public Day(Year year, int day)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(day, 1, nameof(day));
        ArgumentOutOfRangeException.ThrowIfGreaterThan(day, year.DaysIn(), nameof(day));
        Year = year;
        Value = day;
    }

    /// <summary>Every published puzzle day, in year then day order.</summary>
    public static IEnumerable<Day> All() =>
        Enumerable
            .Range(Year.FirstYear, Year.Latest() - Year.FirstYear + 1)
            .Select(y => new Year(y))
            .SelectMany(y => Enumerable.Range(1, y.DaysIn()).Select(d => new Day(y, d)));

    /// <summary>Every published puzzle day that the filter allows.</summary>
    public static IEnumerable<Day> Matching(Filter filter) =>
        All()
            .Where(d =>
                (filter.Year?.Value.Equals(d.Year.Value) ?? true)
                && (filter.Day?.Equals(d.Value) ?? true)
            );

    public override string ToString() => Value.ToString();
}
