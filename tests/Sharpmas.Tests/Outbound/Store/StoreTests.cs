using Sharpmas.Domain.Address;
using Sharpmas.Outbound.Client;
using Sharpmas.Outbound.Store;

// Aliased because this test namespace ends in Store, and outside a namespace
// declaration the namespace name wins over a plain type of the same name.
using Cache = Sharpmas.Outbound.Store.Store;

namespace Sharpmas.Tests.Outbound.Store;

/// <summary>A parseable cookie standing in for a real one.</summary>
static class Cookies
{
    public static SessionCookie Of(char fill) =>
        SessionCookie.Parse(new string(fill, SessionCookie.Length));
}

public class InputTests
{

    [Fact]
    public void RecognisesItsOwnSession()
    {
        var input = Input.Fetched(Cookies.Of('a'), "()()");
        Assert.True(input.IsFrom(Cookies.Of('a')));
        Assert.False(input.IsFrom(Cookies.Of('b')));
    }

    /// <summary>The cookie itself never reaches disk, only a digest of it.</summary>
    [Fact]
    public void StoresADigestRatherThanTheCookie()
    {
        var cookie = Cookies.Of('c');
        var input = Input.Fetched(cookie, "()()");
        Assert.DoesNotContain(cookie.Value, input.Hash);
        Assert.Equal(64, input.Hash.Length);
    }

    [Fact]
    public void FromPartsKeepsTheStoredHash()
    {
        var written = Input.Fetched(Cookies.Of('d'), "()()");
        var read = Input.FromParts(written.Hash, written.Data);
        Assert.True(read.IsFrom(Cookies.Of('d')));
    }
}

public class StoreTests : IDisposable
{
    /// <summary>A root per test, so nothing touches the real cache.</summary>
    readonly string root = Path.Combine(
        Path.GetTempPath(),
        $"sharpmas-test-{Guid.NewGuid():N}"
    );

    readonly Day day = new(new Year(2015), 1);

    public void Dispose()
    {
        if (Directory.Exists(root))
        {
            Directory.Delete(root, recursive: true);
        }
        GC.SuppressFinalize(this);
    }

    static Entry Entry(SessionCookie cookie, string? partTwo = "## two") =>
        new()
        {
            Input = Input.Fetched(cookie, "()()"),
            Instructions = new Instructions { PartOne = "## one", PartTwo = partTwo },
        };

    [Fact]
    public void RoundTrips()
    {
        Assert.Null(Cache.ReadEntryIn(root, day));

        Cache.WriteEntryIn(root, day, Entry(Cookies.Of('d')));
        var read = Cache.ReadEntryIn(root, day);

        Assert.NotNull(read);
        Assert.Equal("()()", read.Input.Data);
        Assert.True(read.Input.IsFrom(Cookies.Of('d')));
        Assert.Equal("## one", read.Instructions.PartOne);
        Assert.Equal("## two", read.Instructions.PartTwo);
    }

    /// <summary>Zero padded so a directory listing sorts the way a human reads it.</summary>
    [Fact]
    public void PadsTheDay()
    {
        Cache.WriteEntryIn(root, day, Entry(Cookies.Of('d')));
        Assert.True(Directory.Exists(Path.Combine(root, "2015", "01")));
    }

    [Fact]
    public void MissingPartTwoReadsAsNull()
    {
        Cache.WriteEntryIn(root, day, Entry(Cookies.Of('d'), partTwo: null));
        Assert.Null(Cache.ReadEntryIn(root, day)!.Instructions.PartTwo);
    }

    /// <summary>A blank file is half-written, so it reads as absent.</summary>
    [Fact]
    public void BlankPartTwoReadsAsMissing()
    {
        Cache.WriteEntryIn(root, day, Entry(Cookies.Of('d')));
        File.WriteAllText(Path.Combine(root, "2015", "01", "part_two.md"), "  \n");
        Assert.Null(Cache.ReadEntryIn(root, day)!.Instructions.PartTwo);
    }

    /// <summary>An input nothing can vouch for is one to fetch again.</summary>
    [Fact]
    public void EntryWithoutASessionReadsAsMissing()
    {
        Cache.WriteEntryIn(root, day, Entry(Cookies.Of('d')));
        File.Delete(Path.Combine(root, "2015", "01", "session"));
        Assert.Null(Cache.ReadEntryIn(root, day));
    }

    [Fact]
    public void EnsureDirRefusesToClobberAFile()
    {
        Directory.CreateDirectory(root);
        var path = Path.Combine(root, "in-the-way");
        File.WriteAllText(path, "");

        Assert.Throws<InvalidOperationException>(() => Cache.EnsureDir(path));
    }
}
