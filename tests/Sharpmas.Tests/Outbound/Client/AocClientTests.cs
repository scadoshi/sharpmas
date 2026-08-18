namespace Sharpmas.Tests.Outbound.Client;

using Sharpmas.Domain.Solution;
using Sharpmas.Outbound.Client;

public class VerdictFromTests
{
    // Fixtures are the real replies AOC gave for 2015 day 1 on a scratch
    // account, trimmed to the sentence that carries the verdict.
    const string Correct =
        "That's the right answer!  You are <span class=\"day-success\">one gold star</span> "
        + "closer to powering the weather machine.";
    const string High =
        "That's not the right answer; your answer is too high.  If you're stuck, make sure "
        + "you're using the full input data";
    const string Low =
        "That's not the right answer; your answer is too low.  If you're stuck, make sure "
        + "you're using the full input data";
    const string Wrong =
        "That's not the right answer.  If you're stuck, make sure you're using the full "
        + "input data; there are also some general tips";
    const string Cooldown =
        "You gave an answer too recently; you have to wait after submitting an answer "
        + "before trying again.  You have 1m 0s left to wait.";
    const string Solved =
        "You don't seem to be solving the right level.  Did you already complete it?";

    [Fact]
    public void ClassifiesReplies()
    {
        Assert.IsType<AocVerdict.Correct>(AocClient.VerdictFrom(Correct));
        Assert.IsType<AocVerdict.High>(AocClient.VerdictFrom(High));
        Assert.IsType<AocVerdict.Low>(AocClient.VerdictFrom(Low));
        Assert.IsType<AocVerdict.Incorrect>(AocClient.VerdictFrom(Wrong));
        Assert.IsType<AocVerdict.AlreadySolved>(AocClient.VerdictFrom(Solved));
        Assert.IsType<AocVerdict.Cooldown>(AocClient.VerdictFrom(Cooldown));
    }

    /// <summary>A directional reply also contains the generic phrase, so order matters.</summary>
    [Fact]
    public void DirectionBeatsGeneric()
    {
        Assert.Contains("That's not the right answer", High);
        Assert.IsType<AocVerdict.High>(AocClient.VerdictFrom(High));
    }

    [Fact]
    public void ExtractsWait()
    {
        Assert.Equal("1m 0s", AocClient.WaitFrom(Cooldown));
        Assert.Equal("unknown", AocClient.WaitFrom("nothing here"));
    }
}

public class SolverVerdictTests
{
    /// <summary>
    /// Read from our side, so a negative comparison means ours was the low one.
    /// </summary>
    [Fact]
    public void OrderingReadsFromOurSide()
    {
        Assert.IsType<SolverVerdict.Low>(SolverVerdict.From(-1));
        Assert.IsType<SolverVerdict.Correct>(SolverVerdict.From(0));
        Assert.IsType<SolverVerdict.High>(SolverVerdict.From(1));
    }

    /// <summary>
    /// A comparison only promises a sign, never a particular value, so anything
    /// negative has to read as low.
    /// </summary>
    [Fact]
    public void NormalisesAnySign()
    {
        Assert.IsType<SolverVerdict.Low>(SolverVerdict.From(-42));
        Assert.IsType<SolverVerdict.High>(SolverVerdict.From(42));
    }

    [Fact]
    public void PlainMatchHasNoDirection()
    {
        Assert.IsType<SolverVerdict.Correct>(SolverVerdict.From(true));
        Assert.IsType<SolverVerdict.Incorrect>(SolverVerdict.From(false));
    }
}
