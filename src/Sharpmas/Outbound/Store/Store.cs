using Sharpmas.Domain.Address;

namespace Sharpmas.Outbound.Store;

/// <summary>Where downloaded things live on disk.</summary>
/// <remarks>
/// One directory per day, every file readable on its own:
///
/// <code>
/// cache/2015/01/input.txt     the puzzle input, verbatim
/// cache/2015/01/session       hash of the cookie that fetched it
/// cache/2015/01/part_one.md   puzzle text
/// cache/2015/01/part_two.md   puzzle text, absent until part one is solved
/// </code>
///
/// Plain files rather than one structured document, because an input and a page
/// of puzzle text both read badly escaped onto a single JSON line.
/// </remarks>
public static class Store
{
    const string InputFile = "input.txt";
    const string SessionFile = "session";
    const string PartOneFile = "part_one.md";
    const string PartTwoFile = "part_two.md";

    /// <summary>A day's directory: year then zero-padded day.</summary>
    /// <remarks>Padded so a directory listing sorts the way a human reads it.</remarks>
    static string DayPath(string root, Day day) =>
        Path.Combine(root, day.Year.Value.ToString(), day.Value.ToString("00"));

    /// <summary>Reads a day's cache, or null when nothing has been downloaded.</summary>
    /// <remarks>
    /// Returns what is on disk whatever session it came from, but a missing
    /// session file reads as null: an input nothing can vouch for is one to
    /// fetch again.
    /// </remarks>
    public static Entry? ReadEntry(Day day) => ReadEntryIn(Paths.CacheRoot, day);

    /// <summary>Reads from an arbitrary cache root, so tests need no real cache.</summary>
    public static Entry? ReadEntryIn(string root, Day day)
    {
        var dir = DayPath(root, day);

        var data = ReadOptional(Path.Combine(dir, InputFile));
        var hash = ReadOptional(Path.Combine(dir, SessionFile));
        var partOne = ReadOptional(Path.Combine(dir, PartOneFile));

        if (data is null || hash is null || partOne is null)
        {
            return null;
        }

        return new Entry
        {
            Input = Input.FromParts(hash.Trim(), data),
            Instructions = new Instructions
            {
                PartOne = partOne,
                PartTwo = ReadOptional(Path.Combine(dir, PartTwoFile)),
            },
        };
    }

    /// <summary>Writes a day's cache, creating the directory if needed.</summary>
    public static void WriteEntry(Day day, Entry entry) =>
        WriteEntryIn(Paths.CacheRoot, day, entry);

    /// <summary>Writes to an arbitrary cache root, so tests need no real cache.</summary>
    public static void WriteEntryIn(string root, Day day, Entry entry)
    {
        var dir = DayPath(root, day);
        EnsureDir(dir);

        File.WriteAllText(Path.Combine(dir, InputFile), entry.Input.Data);
        File.WriteAllText(Path.Combine(dir, SessionFile), entry.Input.Hash);
        File.WriteAllText(Path.Combine(dir, PartOneFile), entry.Instructions.PartOne);
        if (entry.Instructions.PartTwo is string partTwo)
        {
            File.WriteAllText(Path.Combine(dir, PartTwoFile), partTwo);
        }
    }

    /// <summary>Reads a file, or null when it is missing or blank.</summary>
    /// <remarks>
    /// Blank counts as missing so a half-written file reads as absent and is
    /// fetched again, rather than as content nothing will ever replace.
    /// </remarks>
    static string? ReadOptional(string path)
    {
        if (!File.Exists(path))
        {
            return null;
        }
        var contents = File.ReadAllText(path);
        return string.IsNullOrWhiteSpace(contents) ? null : contents;
    }

    /// <summary>Creates a directory and its parents, refusing to clobber a file.</summary>
    public static void EnsureDir(string path)
    {
        if (Directory.Exists(path))
        {
            return;
        }
        if (File.Exists(path))
        {
            throw new InvalidOperationException($"path exists but is not a dir: {path}");
        }
        Directory.CreateDirectory(path);
    }
}
