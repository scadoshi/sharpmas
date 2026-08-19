using Sharpmas.Domain.Solution.Common;

namespace Sharpmas.Tests.Domain.Solution.Common;

public class CellTests
{
    /// <summary>Up decreases the row: a grid counts down from the top left.</summary>
    [Fact]
    public void MovesOneStepEachWay()
    {
        Assert.Equal(Cell.Origin, new Cell(0, 1).SaturatingMoved(Direction.Left, 1));
        Assert.Equal(new(0, 1), Cell.Origin.SaturatingMoved(Direction.Right, 1));
        Assert.Equal(Cell.Origin, new Cell(1, 0).SaturatingMoved(Direction.Up, 1));
        Assert.Equal(new(1, 0), Cell.Origin.SaturatingMoved(Direction.Down, 1));

        Assert.Equal(Cell.Origin, new Cell(0, 1).CheckedMoved(Direction.Left, 1));
        Assert.Equal(new(0, 1), Cell.Origin.CheckedMoved(Direction.Right, 1));
        Assert.Equal(Cell.Origin, new Cell(1, 0).CheckedMoved(Direction.Up, 1));
        Assert.Equal(new(1, 0), Cell.Origin.CheckedMoved(Direction.Down, 1));
    }

    /// <summary>Both ends clamp. The floor matters most: unsigned subtraction wraps.</summary>
    [Fact]
    public void SaturatingMovedPastTheEdgeClamps()
    {
        Assert.Equal(uint.MinValue, Cell.Origin.SaturatingMoved(Direction.Left, 1).Column);
        Assert.Equal(
            uint.MaxValue,
            new Cell(0, uint.MaxValue).SaturatingMoved(Direction.Right, 1).Column
        );
        Assert.Equal(uint.MinValue, Cell.Origin.SaturatingMoved(Direction.Up, 1).Row);
        Assert.Equal(
            uint.MaxValue,
            new Cell(uint.MaxValue, 0).SaturatingMoved(Direction.Down, 1).Row
        );
    }

    [Fact]
    public void CheckedMovedPastTheEdgeReturnsNull()
    {
        Assert.Null(Cell.Origin.CheckedMoved(Direction.Left, 1));
        Assert.Null(new Cell(0, uint.MaxValue).CheckedMoved(Direction.Right, 1));
        Assert.Null(Cell.Origin.CheckedMoved(Direction.Up, 1));
        Assert.Null(new Cell(uint.MaxValue, 0).CheckedMoved(Direction.Down, 1));
    }

    /// <summary>Guards the value equality a HashSet walk depends on.</summary>
    [Fact]
    public void SamePlaceMeansEqual()
    {
        Assert.Equal(new Cell(2, 3), new Cell(2, 3));
        Assert.NotEqual(new Cell(1, 2), new Cell(3, 4));

        var visited = new HashSet<Cell> { new(2, 3) };
        Assert.False(visited.Add(new Cell(2, 3)));
        Assert.True(visited.Add(new Cell(3, 2)));
    }
}
