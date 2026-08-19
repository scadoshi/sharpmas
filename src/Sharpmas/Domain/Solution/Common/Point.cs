namespace Sharpmas.Domain.Solution.Common;

/// <summary>A signed position on the cartesian plane, y growing upward.</summary>
/// <remarks>
/// A record struct, so two points at the same place are equal and hash alike
/// without any of it being written by hand. That is what lets a walk record
/// where it has been in a <see cref="HashSet{T}"/>.
///
/// For row and column indices into a grid, a separate cell type counts rows
/// downward instead.
/// </remarks>
public readonly record struct Point(int X, int Y)
{
    /// <summary>The origin, which is where a walk starts.</summary>
    public static Point Origin => new(0, 0);

    /// <summary>Moves a distance in a direction, clamping at the edges of int.</summary>
    /// <remarks>
    /// Clamps rather than wrapping, because a walk that runs off the end of the
    /// number line is already wrong and wrapping would hide it behind a
    /// plausible answer. The arithmetic happens in long so the comparison
    /// itself cannot overflow.
    /// </remarks>
    public Point SaturatingMoved(Direction direction, int distance) =>
        direction switch
        {
            Direction.Left => this with { X = int.CreateSaturating((long)X - distance) },
            Direction.Right => this with { X = int.CreateSaturating((long)X + distance) },
            Direction.Down => this with { Y = int.CreateSaturating((long)Y - distance) },
            Direction.Up => this with { Y = int.CreateSaturating((long)Y + distance) },
            _ => this,
        };

    /// <summary>Manhattan distance back to the origin, not the straight line.</summary>
    /// <remarks>
    /// Returns a long because two int magnitudes can sum past int, and because
    /// negating int.MinValue overflows where widening it does not.
    /// </remarks>
    public long DistanceFromOrigin() => Math.Abs((long)X) + Math.Abs((long)Y);
}
