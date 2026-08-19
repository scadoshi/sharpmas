using Sharpmas.Domain.Solution.Common;

namespace Sharpmas.Tests.Domain.Solution.Common;

public class TurnTests
{
    [Fact]
    public void ReversesEitherWay()
    {
        Assert.Equal(Turn.Right, Turn.Left.Reversed);
        Assert.Equal(Turn.Left, Turn.Right.Reversed);
    }

    [Theory]
    [InlineData("l", Turn.Left)]
    [InlineData("L", Turn.Left)]
    [InlineData("left", Turn.Left)]
    [InlineData(" RIGHT ", Turn.Right)]
    [InlineData("r", Turn.Right)]
    public void ReadsLettersAndWords(string text, Turn expected)
    {
        Assert.Equal(expected, Turn.Parse(text));
    }

    /// <summary>
    /// A direction cannot become a turn, which is the reason this type is
    /// separate: an instruction like U3 read as a direction turns nowhere and
    /// walks three.
    /// </summary>
    [Theory]
    [InlineData("u")]
    [InlineData("up")]
    [InlineData("down")]
    [InlineData("")]
    public void RefusesAnythingThatIsNotATurn(string text)
    {
        Assert.False(Turn.TryParse(text, out _));
        Assert.Throws<FormatException>(() => Turn.Parse(text));
    }
}

public class DirectionTests
{
    [Fact]
    public void TurnsClockwise()
    {
        Assert.Equal(Direction.Right, Direction.Up.TurnedRight);
        Assert.Equal(Direction.Down, Direction.Right.TurnedRight);
        Assert.Equal(Direction.Left, Direction.Down.TurnedRight);
        Assert.Equal(Direction.Up, Direction.Left.TurnedRight);
    }

    [Fact]
    public void TurnsAnticlockwise()
    {
        Assert.Equal(Direction.Left, Direction.Up.TurnedLeft);
        Assert.Equal(Direction.Down, Direction.Left.TurnedLeft);
        Assert.Equal(Direction.Right, Direction.Down.TurnedLeft);
        Assert.Equal(Direction.Up, Direction.Right.TurnedLeft);
    }

    [Fact]
    public void FourTurnsReturnToTheStart()
    {
        foreach (var start in Direction.All)
        {
            var turned = start;
            for (var i = 0; i < 4; i++)
            {
                turned = turned.TurnedRight;
            }
            Assert.Equal(start, turned);
        }
    }

    [Fact]
    public void TurnedFollowsTheTurn()
    {
        Assert.Equal(Direction.Up.TurnedLeft, Direction.Up.Turned(Turn.Left));
        Assert.Equal(Direction.Up.TurnedRight, Direction.Up.Turned(Turn.Right));
    }

    [Fact]
    public void ReversedIsTwoQuarterTurns()
    {
        Assert.Equal(Direction.Down, Direction.Up.Reversed);
        Assert.Equal(Direction.Left, Direction.Right.Reversed);
        Assert.All(Direction.All, d => Assert.Equal(d, d.Reversed.Reversed));
    }

    [Theory]
    [InlineData("u", Direction.Up)]
    [InlineData("UP", Direction.Up)]
    [InlineData(" left ", Direction.Left)]
    [InlineData("R", Direction.Right)]
    [InlineData("down", Direction.Down)]
    public void ReadsLettersAndWords(string text, Direction expected)
    {
        Assert.Equal(expected, Direction.Parse(text));
    }

    [Theory]
    [InlineData("")]
    [InlineData("north")]
    [InlineData("x")]
    public void RefusesAnythingElse(string text)
    {
        Assert.False(Direction.TryParse(text, out _));
        Assert.Throws<FormatException>(() => Direction.Parse(text));
    }
}
