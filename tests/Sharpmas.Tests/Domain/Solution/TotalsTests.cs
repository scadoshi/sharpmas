using Sharpmas.Domain.Address;
using Sharpmas.Domain.Solution;

namespace Sharpmas.Tests.Domain.Solution;

public class TotalsTests
{
    static Day At(int year, int day)
    {
        return new Day(new Year(year), day);
    }

    static TimeSpan Micros(int count)
    {
        return TimeSpan.FromMicroseconds(count);
    }

    /// <summary>Both parts answered, so both count.</summary>
    static Solved AnsweredBoth(TimeSpan parsed, TimeSpan one, TimeSpan two)
    {
        return new Solved
        {
            ParsedIn = parsed,
            PartOne = new Outcome(new AnswerResult.Ok(new Answer.Value("a")), one),
            PartTwo = new Outcome(new AnswerResult.Ok(new Answer.Value("b")), two),
        };
    }

    [Fact]
    public void NothingRunHasNoMeans()
    {
        var totals = new Totals();

        Assert.Equal(0, totals.Days);
        Assert.Null(totals.ParseMean);
        Assert.Null(totals.SolveMean);
    }

    /// <summary>
    /// The count a caller gates on. One day is not a summary of anything.
    /// </summary>
    [Fact]
    public void OneDayIsOneDay()
    {
        var totals = new Totals();
        totals.Add(At(2015, 1), AnsweredBoth(Micros(2), Micros(10), Micros(30)));

        Assert.Equal(1, totals.Days);
        Assert.Equal(Micros(2), totals.ParseMean);
        Assert.Equal(Micros(20), totals.SolveMean);
    }

    [Fact]
    public void TotalsAndMeansSpanEveryDay()
    {
        var totals = new Totals();
        totals.Add(At(2015, 1), AnsweredBoth(Micros(2), Micros(10), Micros(30)));
        totals.Add(At(2015, 2), AnsweredBoth(Micros(4), Micros(50), Micros(70)));

        Assert.Equal(Micros(3), totals.ParseMean);
        Assert.Equal(Micros(40), totals.SolveMean);
    }

    /// <summary>
    /// The whole reason for <c>SolveTime</c>: a part with no answer did no work,
    /// so counting it would report a mean of a measurement of nothing.
    /// </summary>
    [Fact]
    public void OnlyAnsweredPartsReachTheSolvingTotal()
    {
        var totals = new Totals();
        totals.Add(
            At(2015, 25),
            new Solved
            {
                ParsedIn = Micros(2),
                PartOne = new Outcome(new AnswerResult.Ok(new Answer.Value("a")), Micros(10)),
                PartTwo = new Outcome(new AnswerResult.Ok(new Answer.None()), Micros(6)),
            }
        );

        Assert.Equal(Micros(10), totals.SolveMean);
    }

    /// <summary>
    /// With nothing solved, the solving mean and slowest part lines are absent
    /// rather than zero.
    /// </summary>
    [Fact]
    public void StubDaysCountTowardsParsingAlone()
    {
        var totals = new Totals();
        foreach (var year in new[] { 2016, 2017 })
        {
            totals.Add(
                At(year, 2),
                new Solved
                {
                    ParsedIn = Micros(8),
                    PartOne = new Outcome(
                        new AnswerResult.Ok(new Answer.Unwritten()),
                        TimeSpan.Zero
                    ),
                    PartTwo = new Outcome(new AnswerResult.Err(new Exception("broke")), Micros(99)),
                }
            );
        }

        Assert.Equal(Micros(8), totals.ParseMean);
        Assert.Null(totals.SolveMean);
        Assert.Equal(
            $"total time spent parsing: 16µs{Environment.NewLine}"
                + $"average parse time per day: 8µs{Environment.NewLine}"
                + $"total time spent solving: 0ns{Environment.NewLine}",
            totals.ToString()
        );
    }

    [Fact]
    public void TheSlowestPartIsNamed()
    {
        var totals = new Totals();
        totals.Add(At(2015, 1), AnsweredBoth(Micros(1), Micros(10), Micros(30)));
        totals.Add(At(2021, 4), AnsweredBoth(Micros(1), Micros(90), Micros(20)));
        totals.Add(At(2024, 7), AnsweredBoth(Micros(1), Micros(15), Micros(15)));

        Assert.EndsWith(
            $"slowest part: year 2021 day 4 part one [90µs]{Environment.NewLine}",
            totals.ToString()
        );
    }
}
