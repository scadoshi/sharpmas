namespace Sharpmas.Tests.Outbound.Store;

using Sharpmas.Domain.Address;
using Sharpmas.Outbound.Store;
using Store = Sharpmas.Outbound.Store.Store;

public class InputTests
{
    [Fact]
    public void RecognisesItsOwnSession()
    {
        var input = Input.Fetched("cookie-a", "()()");
        Assert.True(input.IsFrom("cookie-a"));
        Assert.False(input.IsFrom("cookie-b"));
    }

    /// <summary>The cookie itself never reaches disk, only a digest of it.</summary>
    [Fact]
    public void StoresADigestRatherThanTheCookie()
    {
        var input = Input.Fetched("secret", "()()");
        Assert.DoesNotContain("secret", input.Hash);
        Assert.Equal(64, input.Hash.Length);
    }

    [Fact]
    public void FromPartsKeepsTheStoredHash()
    {
        var written = Input.Fetched("cookie", "()()");
        var read = Input.FromParts(written.Hash, written.Data);
        Assert.True(read.IsFrom("cookie"));
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

    static Entry Entry(string cookie, string? partTwo = "## two") =>
        new()
        {
            Input = Input.Fetched(cookie, "()()"),
            Instructions = new Instructions { PartOne = "## one", PartTwo = partTwo },
        };

    [Fact]
    public void RoundTrips()
    {
        Assert.Null(Store.ReadEntryIn(root, day));

        Store.WriteEntryIn(root, day, Entry("cookie"));
        var read = Store.ReadEntryIn(root, day);

        Assert.NotNull(read);
        Assert.Equal("()()", read.Input.Data);
        Assert.True(read.Input.IsFrom("cookie"));
        Assert.Equal("## one", read.Instructions.PartOne);
        Assert.Equal("## two", read.Instructions.PartTwo);
    }

    /// <summary>Zero padded so a directory listing sorts the way a human reads it.</summary>
    [Fact]
    public void PadsTheDay()
    {
        Store.WriteEntryIn(root, day, Entry("cookie"));
        Assert.True(Directory.Exists(Path.Combine(root, "2015", "01")));
    }

    [Fact]
    public void MissingPartTwoReadsAsNull()
    {
        Store.WriteEntryIn(root, day, Entry("cookie", partTwo: null));
        Assert.Null(Store.ReadEntryIn(root, day)!.Instructions.PartTwo);
    }

    /// <summary>
    /// A blank file is a half-written one, so it reads as absent and gets
    /// fetched again rather than counting as content.
    /// </summary>
    [Fact]
    public void BlankPartTwoReadsAsMissing()
    {
        Store.WriteEntryIn(root, day, Entry("cookie"));
        File.WriteAllText(Path.Combine(root, "2015", "01", "part_two.md"), "  \n");
        Assert.Null(Store.ReadEntryIn(root, day)!.Instructions.PartTwo);
    }

    /// <summary>An input nothing can vouch for is one to fetch again.</summary>
    [Fact]
    public void EntryWithoutASessionReadsAsMissing()
    {
        Store.WriteEntryIn(root, day, Entry("cookie"));
        File.Delete(Path.Combine(root, "2015", "01", "session"));
        Assert.Null(Store.ReadEntryIn(root, day));
    }

    [Fact]
    public void EnsureDirRefusesToClobberAFile()
    {
        Directory.CreateDirectory(root);
        var path = Path.Combine(root, "in-the-way");
        File.WriteAllText(path, "");

        Assert.Throws<InvalidOperationException>(() => Store.EnsureDir(path));
    }
}
