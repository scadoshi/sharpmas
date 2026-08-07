namespace Sharpmas.Domain.Address;

/// <summary>
/// A validated puzzle day within a validated <see cref="Address.Year"/>.
/// </summary>
/// <remarks>
/// A day always carries its year, so it names one puzzle unambiguously. The
/// constructor takes a built <see cref="Address.Year"/> rather than a number, so a
/// day cannot exist without a year having been validated first.
/// </remarks>
public sealed class Day
{
    /// <summary>
    /// The day of the event, counting from 1.
    /// </summary>
    public int Value { get; }

    /// <summary>
    /// The event this day belongs to.
    /// </summary>
    public Year Year { get; }

    /// <summary>
    /// Creates a validated day.
    /// </summary>
    /// <param name="year">The event the day belongs to.</param>
    /// <param name="day">The day of that event, counting from 1.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when <paramref name="day"/> falls outside the range
    /// <paramref name="year"/> actually published.
    /// </exception>
    public Day(Year year, int day)
    {
        Year = year;
        if (day < 1 || day > Year.DaysIn())
        {
            throw new InvalidOperationException(
                $"invalid day for year {Year.Value}; must be within 1 and {Year.DaysIn()}"
            );
        }
        Value = day;
    }

    /// <summary>
    /// Every published puzzle day, narrowed by the filters.
    /// </summary>
    /// <remarks>
    /// Both arguments are filters rather than a lookup, so passing neither walks
    /// every day of every event, and passing only <paramref name="day"/> walks
    /// that day across every year that had one.
    /// </remarks>
    /// <param name="year">Only this year, or null for all of them.</param>
    /// <param name="day">Only this day, or null for all of them.</param>
    /// <returns>The matching days, in year then day order.</returns>
    public static IEnumerable<Day> Each(int? year, int? day)
    {
        return Enumerable
            .Range(Year.FirstYear, Year.Latest() - Year.FirstYear + 1)
            .Where(y => year?.Equals(y) ?? true)
            .Select(y => new Year(y))
            .SelectMany(y =>
                Enumerable
                    .Range(1, y.DaysIn())
                    .Where(d => day?.Equals(d) ?? true)
                    .Select(d => new Day(y, d))
            );
    }
}
