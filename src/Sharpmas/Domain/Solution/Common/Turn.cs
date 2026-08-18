using System.Diagnostics;

namespace Sharpmas.Domain.Solution.Common;

/// <summary>A quarter turn, either way.</summary>
/// <remarks>
/// Separate from <see cref="Direction"/> so that nothing has to accept
/// <c>Up</c> and <c>Down</c> where they name no turn. An instruction reading
/// <c>U3</c> parsed as a direction turns nowhere and walks three, which is a
/// bug this type exists to make unspellable.
/// </remarks>
public enum Turn
{
    Left,
    Right,
}

/// <summary>Extensions on <see cref="Turn"/>.</summary>
public static class TurnExtensions
{
    extension(Turn turn)
    {
        /// <summary>The turn the other way.</summary>
        public Turn Reversed =>
            turn switch
            {
                Turn.Left => Turn.Right,
                Turn.Right => Turn.Left,
                _ => throw new UnreachableException($"unhandled {nameof(Turn)}: {turn}"),
            };
    }

    extension(Turn)
    {
        /// <summary>Reads a turn from a letter or the full word, either case.</summary>
        public static bool TryParse(string value, out Turn turn)
        {
            switch (value.Trim().ToLowerInvariant())
            {
                case "l" or "left":
                    turn = Turn.Left;
                    return true;
                case "r" or "right":
                    turn = Turn.Right;
                    return true;
                default:
                    turn = default;
                    return false;
            }
        }

        /// <summary>Reads a turn, failing rather than defaulting.</summary>
        /// <exception cref="FormatException">Thrown when the text names no turn.</exception>
        public static Turn Parse(string value) =>
            Turn.TryParse(value, out var turn)
                ? turn
                : throw new FormatException($"invalid turn: {value}");
    }
}
