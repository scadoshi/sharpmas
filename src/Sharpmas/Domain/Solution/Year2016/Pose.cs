using Sharpmas.Domain.Solution.Common;

namespace Sharpmas.Domain.Solution.Year2016;

/// <summary>A position plus a heading, starting at the origin facing up.</summary>
/// <remarks>
/// A point alone will not do, since R2 turns relative to wherever the last
/// instruction left you pointing.
/// </remarks>
public readonly record struct Pose(Direction Heading, Point Position)
{
    /// <summary>At the origin, facing up.</summary>
    public static Pose Start => new(Direction.Up, Point.Origin);

    /// <summary>Walks along the current heading, clamping at the int edges.</summary>
    public Pose SaturatingMoved(int distance) =>
        this with { Position = Position.SaturatingMoved(Heading, distance) };

    /// <summary>Turns without moving.</summary>
    public Pose Turned(Turn turn) => this with { Heading = Heading.Turned(turn) };
}
