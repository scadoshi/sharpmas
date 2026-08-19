using Sharpmas.Domain.Solution.Common;

namespace Sharpmas.Tests.Domain.Solution.Common;

public class PointTests
{
    [Fact]
    public void MovesOneStepEachWay()
    {
        Assert.Equal(new Point(-1, 0), Point.Origin.SaturatingMoved(Direction.Left, 1));
        Assert.Equal(new Point(1, 0), Point.Origin.SaturatingMoved(Direction.Right, 1));
        Assert.Equal(new Point(0, 1), Point.Origin.SaturatingMoved(Direction.Up, 1));
        Assert.Equal(new Point(0, -1), Point.Origin.SaturatingMoved(Direction.Down, 1));
    }

    /// <summary>
    /// Clamps rather than wrapping. Wrapping would turn a walk that ran off the
    /// number line into a plausible wrong answer instead of an obvious one.
    /// </summary>
    [Fact]
    public void MovingPastTheEdgeClamps()
    {
        Assert.Equal(int.MinValue, new Point(int.MinValue, 0).SaturatingMoved(Direction.Left, 1).X);
        Assert.Equal(int.MaxValue, new Point(int.MaxValue, 0).SaturatingMoved(Direction.Right, 1).X);
        Assert.Equal(int.MaxValue, new Point(0, int.MaxValue).SaturatingMoved(Direction.Up, 1).Y);
        Assert.Equal(int.MinValue, new Point(0, int.MinValue).SaturatingMoved(Direction.Down, 1).Y);
    }

    /// <summary>A distance large enough to overflow still lands on the edge.</summary>
    [Fact]
    public void MovingAnEnormousDistanceClamps()
    {
        Assert.Equal(int.MaxValue, new Point(1, 0).SaturatingMoved(Direction.Right, int.MaxValue).X);
        Assert.Equal(int.MinValue, new Point(-1, 0).SaturatingMoved(Direction.Left, int.MaxValue).X);
    }

    [Fact]
    public void DistanceFromOriginIsManhattan()
    {
        Assert.Equal(0, Point.Origin.DistanceFromOrigin());
        Assert.Equal(7, new Point(3, 4).DistanceFromOrigin());
        Assert.Equal(7, new Point(-3, -4).DistanceFromOrigin());
    }

    /// <summary>
    /// Math.Abs(int.MinValue) throws, and two int magnitudes can sum past int,
    /// which is why this returns a long.
    /// </summary>
    [Fact]
    public void DistanceFromOriginSurvivesTheExtremes()
    {
        Assert.Equal(2_147_483_648, new Point(int.MinValue, 0).DistanceFromOrigin());
        Assert.Equal(4_294_967_296, new Point(int.MinValue, int.MinValue).DistanceFromOrigin());
    }

    /// <summary>
    /// Part two of 2016 day 1 records where it has been in a HashSet, so two
    /// points at the same place have to be equal and hash alike.
    /// </summary>
    [Fact]
    public void SamePlaceMeansEqual()
    {
        Assert.Equal(new Point(2, 3), new Point(2, 3));
        Assert.NotEqual(new Point(2, 3), new Point(3, 2));

        var visited = new HashSet<Point> { new(2, 3) };
        Assert.False(visited.Add(new Point(2, 3)));
        Assert.True(visited.Add(new Point(3, 2)));
    }
}
