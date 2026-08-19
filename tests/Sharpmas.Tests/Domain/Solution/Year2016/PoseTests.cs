using Sharpmas.Domain.Solution.Common;
using Sharpmas.Domain.Solution.Year2016;

namespace Sharpmas.Tests.Domain.Solution.Year2016;

public class PoseTests
{
    [Fact]
    public void StartsAtTheOriginHeadingUp()
    {
        Assert.Equal(Point.Origin, Pose.Start.Position);
        Assert.Equal(Direction.Up, Pose.Start.Heading);
    }

    [Fact]
    public void TurningChangesTheHeadingAndNothingElse()
    {
        var right = Pose.Start.Turned(Turn.Right);
        Assert.Equal(Direction.Right, right.Heading);
        Assert.Equal(Point.Origin, right.Position);

        var left = Pose.Start.Turned(Turn.Left);
        Assert.Equal(Direction.Left, left.Heading);
        Assert.Equal(Point.Origin, left.Position);
    }

    [Fact]
    public void MovingFollowsTheHeading()
    {
        Assert.Equal(new Point(0, 3), Pose.Start.SaturatingMoved(3).Position);
        Assert.Equal(new Point(3, 0), Pose.Start.Turned(Turn.Right).SaturatingMoved(3).Position);
        Assert.Equal(new Point(-3, 0), Pose.Start.Turned(Turn.Left).SaturatingMoved(3).Position);
    }

    /// <summary>The puzzle's own example: R2, L3 lands 2 east and 3 north, 5 away.</summary>
    [Fact]
    public void WalksThePuzzleExample()
    {
        var pose = Pose
            .Start.Turned(Turn.Right)
            .SaturatingMoved(2)
            .Turned(Turn.Left)
            .SaturatingMoved(3);

        Assert.Equal(new Point(2, 3), pose.Position);
        Assert.Equal(5, pose.Position.DistanceFromOrigin());
    }

    /// <summary>
    /// Four equal sides with a turn between each draws a square, which catches
    /// an ordering mistake in the turns that single steps can miss.
    /// </summary>
    [Fact]
    public void FourRightTurnsReturnToTheStart()
    {
        var pose = Pose.Start;
        for (var i = 0; i < 4; i++)
        {
            pose = pose.Turned(Turn.Right).SaturatingMoved(2);
        }

        Assert.Equal(Point.Origin, pose.Position);
        Assert.Equal(Direction.Up, pose.Heading);
    }

    [Fact]
    public void MovingPastTheEdgeClamps()
    {
        var pose = Pose.Start.SaturatingMoved(int.MaxValue).SaturatingMoved(1);
        Assert.Equal(new Point(0, int.MaxValue), pose.Position);
    }
}
