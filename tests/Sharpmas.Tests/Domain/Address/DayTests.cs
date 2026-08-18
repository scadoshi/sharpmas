namespace Sharpmas.Tests.Domain.Address;

using Sharpmas.Domain.Address;

public class YearTests
{
    [Fact]
    public void RejectsYearsOutsidePublishedEvents()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Year(Year.FirstYear - 1));
        Assert.Throws<ArgumentOutOfRangeException>(() => new Year(Year.Latest() + 1));
        Assert.Equal(Year.FirstYear, new Year(Year.FirstYear).Value);
        Assert.Equal(Year.Latest(), new Year(Year.Latest()).Value);
    }

    /// <summary>2025 ran twelve days, every other event twenty five.</summary>
    [Fact]
    public void KnowsHowManyDaysAnEventPublished()
    {
        Assert.Equal(12, new Year(2025).DaysIn());
        Assert.Equal(25, new Year(2015).DaysIn());
    }
}

public class DayTests
{
    /// <summary>2025 ran twelve days, so day 13 is out of range for that year alone.</summary>
    [Fact]
    public void RejectsDaysTheYearNeverHad()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Day(new Year(2025), 13));
        Assert.Equal(12, new Day(new Year(2025), 12).Value);
        Assert.Equal(13, new Day(new Year(2015), 13).Value);
        Assert.Throws<ArgumentOutOfRangeException>(() => new Day(new Year(2015), 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => new Day(new Year(2015), 26));
    }

    [Fact]
    public void CarriesItsYear()
    {
        var day = new Day(new Year(2016), 3);
        Assert.Equal(2016, day.Year.Value);
        Assert.Equal(3, day.Value);
    }

    [Fact]
    public void EachWalksEveryPublishedDay()
    {
        var all = Day.Each(null, null).ToList();
        var expected = Enumerable
            .Range(Year.FirstYear, Year.Latest() - Year.FirstYear + 1)
            .Sum(y => new Year(y).DaysIn());
        Assert.Equal(expected, all.Count);
    }

    [Fact]
    public void EachFiltersAreIndependent()
    {
        var byYear = Day.Each(2015, null).ToList();
        Assert.Equal(25, byYear.Count);
        Assert.All(byYear, d => Assert.Equal(2015, d.Year.Value));

        var byDay = Day.Each(null, 1).ToList();
        Assert.Equal(Year.Latest() - Year.FirstYear + 1, byDay.Count);
        Assert.All(byDay, d => Assert.Equal(1, d.Value));

        Assert.Single(Day.Each(2015, 1));
    }

    /// <summary>
    /// A day-only filter skips years that never had that day rather than
    /// failing, since 2025 stopped at twelve.
    /// </summary>
    [Fact]
    public void EachSkipsYearsWithoutThatDay()
    {
        var days = Day.Each(null, 25).ToList();
        Assert.All(days, d => Assert.NotEqual(2025, d.Year.Value));
        Assert.Equal(Year.Latest() - Year.FirstYear, days.Count);
    }
}
