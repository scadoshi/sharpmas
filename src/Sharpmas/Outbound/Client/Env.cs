namespace Sharpmas.Outbound.Client;

/// <summary>Environment variables, with the optional .env file folded in.</summary>
public static class Env
{
    const string ContactKey = "CONTACT";
    const string RepoUrlKey = "REPO_URL";
    const string UnconfiguredUserAgent = "sharpmas (unconfigured; set CONTACT in .env)";

    /// <summary>
    /// Reads .env into the environment, leaving any variable already set alone.
    /// </summary>
    /// <remarks>
    /// An exported variable therefore beats the file, which is what makes a
    /// one-off override on the command line work. A missing file is ordinary:
    /// .env is gitignored, so a fresh clone has none.
    /// </remarks>
    static void LoadEnvFile()
    {
        if (!File.Exists(Paths.EnvFile))
        {
            return;
        }
        var lines = File.ReadAllLines(Paths.EnvFile);
        foreach (
            string line in lines.Where(l =>
                !l.Trim().StartsWith('#') && !string.IsNullOrWhiteSpace(l)
            )
        )
        {
            if (!line.Contains('='))
            {
                Console.Error.WriteLine($"ignoring malformed line in .env: {line}");
                continue;
            }
            var parts = line.Split("=", 2);
            var key = parts[0].Trim();
            var value = parts[1].Trim();
            if (Environment.GetEnvironmentVariable(key) is null)
            {
                Environment.SetEnvironmentVariable(key, value);
            }
        }
    }

    /// <summary>Loads the file once, before anything first reads a variable.</summary>
    static Env() => LoadEnvFile();

    /// <summary>The value of `key`, or null when it is unset or blank.</summary>
    /// <remarks>
    /// Blank counts as unset, so `CONTACT=` in the shipped template means what
    /// it looks like rather than producing an empty half of a user agent.
    /// </remarks>
    static string? Set(string key)
    {
        var value = Environment.GetEnvironmentVariable(key)?.Trim();
        return string.IsNullOrWhiteSpace(value) ? null : value;
    }

    /// <summary>The User-Agent, built from REPO_URL and CONTACT, both optional.</summary>
    /// <remarks>
    /// AOC asks automated clients to be reachable. The unconfigured fallback
    /// names nobody on purpose, so a stranger's traffic points at no real
    /// contact and at no other repo.
    /// </remarks>
    public static string UserAgent()
    {
        return (Set(RepoUrlKey), Set(ContactKey)) switch
        {
            (string repo, string contact) => $"{repo} by {contact}",
            (string repo, null) => $"{repo}",
            (null, string contact) => $"sharpmas by {contact}",
            (null, null) => UnconfiguredUserAgent,
        };
    }
}
