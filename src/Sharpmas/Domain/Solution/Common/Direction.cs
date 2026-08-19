using System.Diagnostics;

namespace Sharpmas.Domain.Solution.Common;

/// <summary>One of the four moves along an axis.</summary>
/// <remarks>
/// <c>Up</c> and <c>Down</c> mean opposite things to the two position types:
/// a point counts y upward, a grid cell counts rows down from the top.
///
/// Declared clockwise, which is what makes a right turn one step along the list
/// and a left turn one step back.
/// </remarks>
public enum Direction
{
    Up,
    Right,
    Down,
    Left,
}

/// <summary>Extensions on <see cref="Direction"/>.</summary>
public static class DirectionExtensions
{
    extension(Direction direction)
    {
        /// <summary>A quarter turn clockwise.</summary>
        public Direction TurnedRight =>
            direction switch
            {
                Direction.Up => Direction.Right,
                Direction.Right => Direction.Down,
                Direction.Down => Direction.Left,
                Direction.Left => Direction.Up,
                _ => throw new UnreachableException(
                    $"unhandled {nameof(Direction)}: {direction}"
                ),
            };

        /// <summary>A quarter turn anticlockwise.</summary>
        public Direction TurnedLeft =>
            direction switch
            {
                Direction.Up => Direction.Left,
                Direction.Left => Direction.Down,
                Direction.Down => Direction.Right,
                Direction.Right => Direction.Up,
                _ => throw new UnreachableException(
                    $"unhandled {nameof(Direction)}: {direction}"
                ),
            };

        /// <summary>A quarter turn whichever way the turn says.</summary>
        public Direction Turned(Turn turn) =>
            turn switch
            {
                Turn.Left => direction.TurnedLeft,
                Turn.Right => direction.TurnedRight,
                _ => throw new UnreachableException($"unhandled {nameof(Turn)}: {turn}"),
            };

        /// <summary>The direction facing back the way this one came.</summary>
        public Direction Reversed => direction.TurnedRight.TurnedRight;
    }

    extension(Direction)
    {
        /// <summary>All four, clockwise from up.</summary>
        /// <remarks>
        /// Rebuilt per call, since an extension member cannot hold state. Bind
        /// it to a local before iterating in a hot loop.
        /// </remarks>
        public static IEnumerable<Direction> All =>
            [Direction.Up, Direction.Right, Direction.Down, Direction.Left];

        /// <summary>Reads a direction from a letter or the full word, either case.</summary>
        public static bool TryParse(string value, out Direction direction)
        {
            switch (value.Trim().ToLowerInvariant())
            {
                case "u" or "up":
                    direction = Direction.Up;
                    return true;
                case "r" or "right":
                    direction = Direction.Right;
                    return true;
                case "d" or "down":
                    direction = Direction.Down;
                    return true;
                case "l" or "left":
                    direction = Direction.Left;
                    return true;
                default:
                    direction = default;
                    return false;
            }
        }

        /// <summary>Reads a direction, failing rather than defaulting.</summary>
        public static Direction Parse(string value) =>
            Direction.TryParse(value, out var direction)
                ? direction
                : throw new FormatException($"invalid direction: {value}");
    }
}
