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
            Direction.Left => Column.CheckedSub(distance) is uint moved
                ? this with
                {
                    Column = moved,
                }
                : null,
            Direction.Right => Column.CheckedAdd(distance) is uint moved
                ? this with
                {
                    Column = moved,
                }
                : null,
            Direction.Up => Row.CheckedSub(distance) is uint moved
                ? this with
                {
                    Row = moved,
                }
                : null,
            Direction.Down => Row.CheckedAdd(distance) is uint moved
                ? this with
                {
                    Row = moved,
                }
                : null,
            _ => this,
        };
}
