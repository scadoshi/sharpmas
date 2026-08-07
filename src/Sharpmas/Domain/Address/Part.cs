namespace Sharpmas.Domain.Address;

/// <summary>
/// Which of a day's two puzzles.
/// </summary>
/// <remarks>
/// Naming the part means call sites read <c>Part.One</c> rather than a bare
/// <c>1</c> that could be mistaken for a day number.
/// </remarks>
public sealed class Part
{
    /// <summary>
    /// The two puzzles every day has, except day 25 which has only the first.
    /// </summary>
    public enum PartKind
    {
        /// <summary>The first puzzle, always available.</summary>
        One,

        /// <summary>The second, unlocked by solving the first.</summary>
        Two,
    }

    /// <summary>
    /// Which puzzle this is.
    /// </summary>
    public PartKind Kind { get; set; }

    /// <summary>
    /// The number the wire expects: <c>1</c> or <c>2</c>.
    /// </summary>
    /// <remarks>
    /// Wanted in two places, Advent of Code's <c>level</c> form field and the
    /// solver's URL path, so both read it from here.
    /// </remarks>
    /// <returns>The part number as a string.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <see cref="Kind"/> holds a value outside the enum, which is
    /// possible because a C# enum is named integers rather than a closed set.
    /// </exception>
    public string ToWireValue()
    {
        return Kind switch
        {
            PartKind.One => "1",
            PartKind.Two => "2",
            _ => throw new ArgumentOutOfRangeException(nameof(Kind), Kind, null),
        };
    }
}
