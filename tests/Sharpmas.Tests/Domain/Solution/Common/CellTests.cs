using Sharpmas.Domain.Solution.Common;

namespace Sharpmas.Tests.Domain.Solution.Common;

public class CellTests
{
    /// <summary>
    /// Up decreases the row, since a grid counts down from the top left. That is
    /// the whole reason this type is separate from <see cref="Point"/>.
    /// </summary>
    [Fact]
    public void MovesOneStepEachWay()
    {
        Assert.Equal(Cell.Origin, new Cell(0, 1).SaturatingMoved(Direction.Left, 1));
        Assert.Equal(new(0, 1), Cell.Origin.SaturatingMoved(Direction.Right, 1));
        Assert.Equal(Cell.Origin, new Cell(1, 0).SaturatingMoved(Direction.Up, 1));
        Assert.Equal(new(1, 0), Cell.Origin.SaturatingMoved(Direction.Down, 1));
    }

    /// <summary>
    /// Clamps rather than wrapping, at both ends. The floor is the one that
    /// bites: unsigned subtraction wraps silently, so moving up from row zero
    /// once landed on uint.MaxValue rather than staying put.
    /// </summary>
    [Fact]
    public void MovingPastTheEdgeClamps()
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

    /// <summary>
    /// A grid walk records where it has been in a set, so two cells at the same
    /// index have to be equal and hash alike. C# will not complain if the record
    /// struct becomes a class and this quietly stops holding.
    /// </summary>
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
