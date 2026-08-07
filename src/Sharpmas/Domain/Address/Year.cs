namespace Sharpmas.Domain.Address;

/// <summary>
/// A validated Advent of Code event year, from <see cref="FirstYear"/> through
/// <see cref="Latest"/>.
/// </summary>
/// <remarks>
/// The constructor is the only way in, so holding a <see cref="Year"/> means it
/// names an event that was actually published.
/// </remarks>
public sealed class Year
{
    /// <summary>
    /// The first Advent of Code.
    /// </summary>
    public const int FirstYear = 2015;

    /// <summary>
    /// The actual year value.
    /// </summary>
    public int Value { get; }

    /// <summary>
    /// Creates a validated year.
    /// </summary>
    /// <param name="year">The calendar year of the event.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when <paramref name="year"/> falls outside <see cref="FirstYear"/>
    /// through <see cref="Latest"/>.
    /// </exception>
    public Year(int year)
    {
        if (year < FirstYear || year > Latest())
        {
            throw new InvalidOperationException(
                $"invalid year: must be within {FirstYear} and {Latest()}"
            );
        }
        Value = year;
    }

    /// <summary>
    /// How many puzzle days this event published. Usually 25; 2025 ran 12.
    /// </summary>
    /// <returns>The number of days, counting from 1.</returns>
    public int DaysIn()
    {
        if (Value == 2025)
        {
            return 12;
        }
        return 25;
    }

    /// <summary>
    /// The latest event that has actually been published.
    /// </summary>
    /// <remarks>
    /// A new event drops each December, so for most of the year the current
    /// calendar year has nothing in it yet and the answer is the year before.
    /// </remarks>
    /// <returns>The calendar year of the most recent published event.</returns>
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
}
