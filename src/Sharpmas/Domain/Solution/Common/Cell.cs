namespace Sharpmas.Domain.Solution.Common;

/// <summary>An unsigned grid index, rows counting down from the top left.</summary>
/// <remarks>
/// So <c>Up</c> decreases the row, the opposite of <see cref="Point"/>, which is
/// what to use for signed coordinates on an unbounded plane. Picking the wrong
/// one flips the vertical axis, so a correct walk gives a wrong answer.
///
/// A record struct, so two cells at the same index are equal and hash alike
/// without any of it being written by hand.
/// </remarks>
public record struct Cell(uint Row, uint Column)
{
    /// <summary>The top left corner, which is where a grid walk starts.</summary>
    public static Cell Origin => new(0, 0);

    /// <summary>Moves a distance in a direction, clamping at the edges of the grid.</summary>
    /// <remarks>
    /// Clamps rather than wrapping, since a walk that runs off a grid is already
    /// wrong and wrapping would hide it behind a plausible answer.
    /// </remarks>
    public Cell SaturatingMoved(Direction direction, uint distance) =>
        direction switch
        {
            Direction.Left => this with { Column = uint.CreateSaturating((long)Column - distance) },
            Direction.Right => this with
            {
                Column = uint.CreateSaturating((long)Column + distance),
            },
            Direction.Up => this with { Row = uint.CreateSaturating((long)Row - distance) },
            Direction.Down => this with { Row = uint.CreateSaturating((long)Row + distance) },
            _ => this,
        };

    /// <summary>Moves a distance in a direction, or null if that leaves the grid.</summary>
    /// <remarks>
    /// Clamping suits a walk with no edges; null suits a grid, where leaving it
    /// is the caller's decision rather than a position to carry on from.
    /// </remarks>
    public Cell? CheckedMoved(Direction direction, uint distance) =>
        direction switch
        {
            Direction.Left => Checked((long)Column - distance) is uint moved
                ? this with
                {
                    Column = moved,
                }
                : null,
            Direction.Right => Checked((long)Column + distance) is uint moved
                ? this with
                {
                    Column = moved,
                }
                : null,
            Direction.Up => Checked((long)Row - distance) is uint moved
                ? this with
                {
                    Row = moved,
                }
                : null,
            Direction.Down => Checked((long)Row + distance) is uint moved
                ? this with
                {
                    Row = moved,
                }
                : null,
            _ => this,
        };

    /// <summary>A widened result narrowed back down, or null if it will not fit.</summary>
    /// <remarks>
    /// A comparison rather than a <c>checked</c> block: hitting an edge is
    /// ordinary here, and throwing costs a stack walk on a hot path.
    /// </remarks>
    static uint? Checked(long value) =>
        (value >= uint.MinValue && value <= uint.MaxValue) ? (uint)value : null;
}
