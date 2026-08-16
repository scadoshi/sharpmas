namespace Sharpmas.Tests.Domain.Solution;

using Sharpmas.Domain.Solution;

public class OutcomeTests
{
    private static Outcome Solved(string data = "foo")
    {
        return new Outcome(new AnswerResult.Ok(new Answer.Value(data)), TimeSpan.Zero);
    }

    private static Outcome Failed()
    {
        var cause = new Exception("given string was too short");
        return new Outcome(
            new AnswerResult.Err(new Exception("could not parse the input", cause)),
            TimeSpan.Zero
        );
    }

    [Fact]
    public void BareAnswerHasNoNotes()
    {
        Assert.Equal("foo [0ns]", Solved().ToString());
    }

    [Fact]
    public void SolverVerdictAloneShows()
    {
        var outcome = Solved().WithVerdict(new SolverVerdict.High());
        Assert.Equal("foo (high) [0ns]", outcome.ToString());
    }

    [Fact]
    public void AocVerdictAloneShows()
    {
        var outcome = Solved().WithSubmission(new AocVerdict.Low());
        Assert.Equal("foo (low) [0ns]", outcome.ToString());
    }

    [Fact]
    public void AocCorrectReadsAsANewStar()
    {
        var outcome = Solved().WithSubmission(new AocVerdict.Correct());
        Assert.Equal("foo (new star) [0ns]", outcome.ToString());
    }

    [Fact]
    public void AocAlreadySolvedReadsAsStarred()
    {
        var outcome = Solved().WithSubmission(new AocVerdict.AlreadySolved());
        Assert.Equal("foo (starred) [0ns]", outcome.ToString());
    }

    [Fact]
    public void AocSupersedesTheSolver()
    {
        var starred = Solved()
            .WithVerdict(new SolverVerdict.Correct())
            .WithSubmission(new AocVerdict.AlreadySolved());
        Assert.Equal("foo (starred) [0ns]", starred.ToString());

        var fresh = Solved()
            .WithVerdict(new SolverVerdict.Correct())
            .WithSubmission(new AocVerdict.Correct());
        Assert.Equal("foo (new star) [0ns]", fresh.ToString());
    }

    /// <summary>
    /// Any AOC reply that is not a grade is worth seeing next to what the
    /// solver thought, since neither one settles the answer on its own.
    /// </summary>
    [Fact]
    public void BothVerdictsShowWhenAocDidNotGrade()
    {
        var outcome = Solved()
            .WithVerdict(new SolverVerdict.Correct())
            .WithSubmission(new AocVerdict.Cooldown("1m 0s"));
        Assert.Equal("foo (correct, rate limited, 1m 0s left to wait) [0ns]", outcome.ToString());
    }

    [Fact]
    public void TimingAlwaysRenders()
    {
        var outcome = new Outcome(
            new AnswerResult.Ok(new Answer.Value("foo")),
            TimeSpan.FromMicroseconds(7)
        );
        Assert.EndsWith("[7µs]", outcome.ToString());
    }

    [Fact]
    public void VisualAnswersRenderTheirArt()
    {
        var outcome = new Outcome(new AnswerResult.Ok(new Answer.Visual("###")), TimeSpan.Zero);
        Assert.Contains("###", outcome.ToString());
    }

    [Fact]
    public void AbsentAnswersSaySo()
    {
        var outcome = new Outcome(new AnswerResult.Ok(new Answer.None()), TimeSpan.Zero);
        Assert.Equal("(none) [0ns]", outcome.ToString());
    }

    /// <summary>
    /// The invariant that survived splitting the answer from the outcome:
    /// nothing to submit means nothing to check.
    /// </summary>
    [Fact]
    public void UnsubmittableAnswersNeverTakeAVerdict()
    {
        Answer[] unsubmittable = [new Answer.Visual("art"), new Answer.None()];
        foreach (var answer in unsubmittable)
        {
            var outcome = new Outcome(new AnswerResult.Ok(answer), TimeSpan.Zero)
                .WithVerdict(new SolverVerdict.Correct())
                .WithSubmission(new AocVerdict.Correct());
            Assert.Null(outcome.Verdict);
            Assert.Null(outcome.Submission);
        }
    }

    /// <summary>
    /// The whole chain, since the outermost message rarely names the day.
    /// </summary>
    [Fact]
    public void FailedPartsRenderEveryCause()
    {
        Assert.Equal(
            "error: could not parse the input: given string was too short [0ns]",
            Failed().ToString()
        );
    }

    [Fact]
    public void FailedPartsNeverTakeAVerdict()
    {
        var outcome = Failed()
            .WithVerdict(new SolverVerdict.Correct())
            .WithSubmission(new AocVerdict.Correct());
        Assert.Null(outcome.GetValue());
        Assert.Null(outcome.Verdict);
        Assert.Null(outcome.Submission);
    }
}
