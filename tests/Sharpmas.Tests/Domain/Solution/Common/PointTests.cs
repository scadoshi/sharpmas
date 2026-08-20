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

    /// <summary>Clamping keeps a runaway walk from wrapping into a plausible answer.</summary>
    [Fact]
    public void MovingPastTheEdgeClamps()
    {
        Assert.Equal(int.MinValue, new Point(int.MinValue, 0).SaturatingMoved(Direction.Left, 1).X);
        Assert.Equal(int.MaxValue, new Point(int.MaxValue, 0).SaturatingMoved(Direction.Right, 1).X);
        Assert.Equal(int.MaxValue, new Point(0, int.MaxValue).SaturatingMoved(Direction.Up, 1).Y);
        Assert.Equal(int.MinValue, new Point(0, int.MinValue).SaturatingMoved(Direction.Down, 1).Y);
    }

    [Fact]
    public void CheckedMovesOneStepEachWay()
    {
        Assert.Equal(new Point(-1, 0), Point.Origin.CheckedMoved(Direction.Left, 1));
        Assert.Equal(new Point(1, 0), Point.Origin.CheckedMoved(Direction.Right, 1));
        Assert.Equal(new Point(0, 1), Point.Origin.CheckedMoved(Direction.Up, 1));
        Assert.Equal(new Point(0, -1), Point.Origin.CheckedMoved(Direction.Down, 1));
    }

    [Fact]
    public void CheckedMovedPastTheEdgeReturnsNull()
    {
        Assert.Null(new Point(int.MinValue, 0).CheckedMoved(Direction.Left, 1));
        Assert.Null(new Point(int.MaxValue, 0).CheckedMoved(Direction.Right, 1));
        Assert.Null(new Point(0, int.MaxValue).CheckedMoved(Direction.Up, 1));
        Assert.Null(new Point(0, int.MinValue).CheckedMoved(Direction.Down, 1));
    }

    [Fact]
    public void DistanceFromOriginIsManhattan()
    {
        Assert.Equal(0, Point.Origin.DistanceFromOrigin());
        Assert.Equal(7, new Point(3, 4).DistanceFromOrigin());
        Assert.Equal(7, new Point(-3, -4).DistanceFromOrigin());
    }

    /// <summary>Math.Abs(int.MinValue) throws, and two magnitudes can sum past int.</summary>
    [Fact]
    public void DistanceFromOriginSurvivesTheExtremes()
    {
        Assert.Equal(2_147_483_648, new Point(int.MinValue, 0).DistanceFromOrigin());
        Assert.Equal(4_294_967_296, new Point(int.MinValue, int.MinValue).DistanceFromOrigin());
    }

    /// <summary>Guards the value equality a HashSet walk depends on.</summary>
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
