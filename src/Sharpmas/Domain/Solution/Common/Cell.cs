namespace Sharpmas.Domain.Solution.Common;

/// <summary>An unsigned grid index, rows counting down from the top left.</summary>
/// <remarks>
/// So <c>Up</c> decreases the row, the opposite of <see cref="Point"/>, which is
/// what to use for signed coordinates on an unbounded plane. Picking the wrong
/// one of the two walks a puzzle perfectly and lands somewhere upside down.
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
            Direction.Left => this with { Column = Saturating((long)Column - distance) },
            Direction.Right => this with { Column = Saturating((long)Column + distance) },
            Direction.Up => this with { Row = Saturating((long)Row - distance) },
            Direction.Down => this with { Row = Saturating((long)Row + distance) },
            _ => this,
        };

    /// <summary>Brings a widened result back into range, clamping at both ends.</summary>
    /// <remarks>
    /// Signed, not <c>ulong</c>. Unsigned subtraction wraps silently rather than
    /// throwing, so moving left from column zero would land on
    /// <see cref="uint.MaxValue"/> with nothing to clamp against. A
    /// <see cref="long"/> holds every <see cref="uint"/> with 32 bits to spare,
    /// so neither a sum nor a difference of two can leave it.
    /// </remarks>
    static uint Saturating(long value) => (uint)Math.Clamp(value, uint.MinValue, uint.MaxValue);
}
