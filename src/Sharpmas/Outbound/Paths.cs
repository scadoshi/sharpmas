namespace Sharpmas.Outbound;

/// <summary>Locations derived from the repo, found once at startup.</summary>
public static class Paths
{
    /// <summary>The directory holding sharpmas.slnx.</summary>
    public static string Root { get; } = FindRoot();

    /// <summary>The optional .env file. May not exist.</summary>
    public static string EnvFile { get; } = Path.Combine(Root, ".env");

    static string FindRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "sharpmas.slnx")))
        {
            dir = dir.Parent;
        }
        return dir?.FullName ?? AppContext.BaseDirectory;
    }
}
