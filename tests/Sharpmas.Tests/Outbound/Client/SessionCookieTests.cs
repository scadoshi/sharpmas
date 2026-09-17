using Sharpmas.Outbound.Client;

namespace Sharpmas.Tests.Outbound.Client;

public class SessionCookieTests
{
    static string Valid() => string.Concat(Enumerable.Repeat("a1b2c3d4", SessionCookie.Length / 8));

    [Fact]
    public void AcceptsAFullLengthHexValue()
    {
        Assert.Equal(Valid(), SessionCookie.Parse(Valid()).Value);
    }

    [Fact]
    public void TrimsSurroundingWhitespace()
    {
        Assert.Equal(Valid(), SessionCookie.Parse($"  {Valid()}\n").Value);
    }

    [Fact]
    public void RejectsTheWrongLength()
    {
        Assert.Throws<InvalidOperationException>(() => SessionCookie.Parse(Valid()[1..]));
        Assert.Throws<InvalidOperationException>(() => SessionCookie.Parse(Valid() + "a"));
        Assert.Throws<InvalidOperationException>(() => SessionCookie.Parse(""));
    }

    /// <summary>
    /// The site ignores anything past the first 128 characters, so a longer value
    /// works while reading as a different cookie to the cache.
    /// </summary>
    [Fact]
    public void RejectsAValueTheSiteWouldHaveAccepted()
    {
        Assert.Throws<InvalidOperationException>(() => SessionCookie.Parse(Valid() + "deadbeef"));
    }

    [Fact]
    public void RejectsNonHex()
    {
        Assert.Throws<InvalidOperationException>(() => SessionCookie.Parse("z" + Valid()[1..]));
    }

    [Fact]
    public void SaysHowLongTheValueWas()
    {
        var thrown = Assert.Throws<InvalidOperationException>(() =>
            SessionCookie.Parse(Valid()[1..])
        );
        Assert.Contains($"got {SessionCookie.Length - 1}", thrown.Message);
    }
}
