using Sharpmas.Domain.Solution.Common;
using Sharpmas.Domain.Solution.Year2016.Day01;

namespace Sharpmas.Tests.Domain.Solution.Year2016.Day01;

public class InstructionTests
{
    [Fact]
    public void ParsesATurnAndADistance()
    {
        Assert.Equal(new Instruction(Turn.Right, 2), Instruction.Parse("R2"));
        Assert.Equal(new Instruction(Turn.Left, 347), Instruction.Parse("L347"));
    }

    /// <summary>U3 read as a direction would turn nowhere and walk three.</summary>
    [Theory]
    [InlineData("U3")]
    [InlineData("D3")]
    [InlineData("up3")]
    public void AHeadingIsNotAValidInstruction(string text)
    {
        Assert.Throws<FormatException>(() => Instruction.Parse(text));
    }

    [Theory]
    [InlineData("R")]
    [InlineData("Rx")]
    [InlineData("")]
    public void RejectsAMissingOrUnparseableDistance(string text)
    {
        Assert.Throws<FormatException>(() => Instruction.Parse(text));
    }

    [Fact]
    public void SplitsOnCommasAndTrimsTheSpaces()
    {
        var instructions = Instructions.Parse("R2, L3,R5");
        Assert.Equal(3, instructions.Count);
        Assert.Equal(new Instruction(Turn.Left, 3), instructions[1]);
    }

    /// <summary>A partly parsed walk would give a confidently wrong answer.</summary>
    [Fact]
    public void OneBadInstructionFailsThemAll()
    {
        Assert.Throws<FormatException>(() => Instructions.Parse("R2, U3, L5"));
    }

    [Fact]
    public void EnumeratesInOrder()
    {
        Assert.Equal(
            [Turn.Right, Turn.Left, Turn.Right],
            Instructions.Parse("R2, L3, R5").Select(i => i.Turn)
        );
    }
}
