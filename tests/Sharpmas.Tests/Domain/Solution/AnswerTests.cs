using Sharpmas.Domain.Solution;

namespace Sharpmas.Tests.Domain.Solution;

public class AnswerTests
{
    [Theory]
    [InlineData("", "")]
    [InlineData("foo", "foo")]
    public void ValuePrintsInputPlainly(string input, string expected)
    {
        Assert.Equal(expected, new Answer.Value(input).ToString());
    }

    [Theory]
    [InlineData("", "\n\n")]
    [InlineData("foo", "\nfoo\n")]
    public void VisualSurroundsWithNewLines(string input, string expected)
    {
        Assert.Equal(expected, new Answer.Visual(input).ToString());
    }

    [Fact]
    public void NonePrintsPlainMessage()
    {
        Assert.Equal("(none)", new Answer.None().ToString());
    }
}
