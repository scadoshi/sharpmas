namespace Sharpmas.Domain.Address;

/// <summary>
/// Which of a day's two puzzles.
/// </summary>
/// <remarks>
/// Naming the part means call sites read <c>Part.One</c> rather than a bare
/// <c>1</c> that could be mistaken for a day number.
/// </remarks>
public enum Part
{
    /// <summary>The first puzzle, always available.</summary>
    One,

    /// <summary>The second, unlocked by solving the first.</summary>
    Two,
}

/// <summary>
/// Behaviour hung off <see cref="Part"/>, which cannot carry methods itself.
/// </summary>
public static class PartExtensions
{
    extension(Part part)
    {
        /// <summary>
        /// The number the wire expects: <c>1</c> or <c>2</c>.
        /// </summary>
        /// <remarks>
        /// Wanted in two places, Advent of Code's <c>level</c> form field and
        /// the solver's URL path, so both read it from here.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when the part holds a value outside the enum, which is
        /// possible because a C# enum is named integers rather than a closed
        /// set.
        /// </exception>
        public string WireValue =>
            part switch
            {
                Part.One => "1",
                Part.Two => "2",
                _ => throw new ArgumentOutOfRangeException(nameof(part), part, null),
            };
    }

    extension(Part)
    {
        /// <summary>
        /// Both parts, in order, for walking a day.
        /// </summary>
        /// <remarks>
        /// Day 25 has no second part, but that belongs to the answer rather
        /// than the address, so this stays the full pair.
        /// </remarks>
        public static IEnumerable<Part> All => [Part.One, Part.Two];
    }
}
