using Sharpmas.Inbound;

namespace Sharpmas.Tests.Inbound;

/// <summary>Hand-rolled parser, so nothing else covers these branches.</summary>
public class CliTests
{
    [Fact]
    public void DefaultsToNoFilters()
    {
        var options = Cli.Parse([]);
        Assert.Null(options.Year);
        Assert.Null(options.Day);
        Assert.False(options.Validate);
        Assert.False(options.Submit);
        Assert.False(options.Yes);
    }

    [Theory]
    [InlineData("-y")]
    [InlineData("--year")]
    public void ReadsTheYearEitherWay(string flag)
    {
        Assert.Equal(2015, Cli.Parse([flag, "2015"]).Year);
    }

    [Theory]
    [InlineData("-d")]
    [InlineData("--day")]
    public void ReadsTheDayEitherWay(string flag)
    {
        Assert.Equal(1, Cli.Parse([flag, "1"]).Day);
    }

    [Fact]
    public void ReadsEveryFlagTogether()
    {
        var options = Cli.Parse(["-y", "2015", "-d", "1", "--validate", "--submit", "--yes"]);
        Assert.Equal(2015, options.Year);
        Assert.Equal(1, options.Day);
        Assert.True(options.Validate);
        Assert.True(options.Submit);
        Assert.True(options.Yes);
    }

    /// <summary>A missing value must not read the next flag as its number.</summary>
    [Fact]
    public void RejectsAFlagWithNoValue()
    {
        Assert.Throws<ArgumentException>(() => Cli.Parse(["-y"]));
    }

    [Fact]
    public void RejectsAValueThatIsNotANumber()
    {
        Assert.Throws<ArgumentException>(() => Cli.Parse(["-y", "twenty"]));
    }

    /// <summary>A typo must not be ignored, since ignoring it changes the run.</summary>
    [Fact]
    public void RejectsAnUnknownOption()
    {
        Assert.Throws<ArgumentException>(() => Cli.Parse(["--submitt"]));
    }
}
