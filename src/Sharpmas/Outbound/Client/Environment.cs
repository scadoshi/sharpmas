namespace Sharpmas.Outbound.Client;

/// <summary>Environment variables, with the optional .env file folded in.</summary>
public static class Environment
{
    const string ContactKey = "CONTACT";
    const string RepoUrlKey = "REPO_URL";
    const string CookieKey = "COOKIE";
    const string UnconfiguredUserAgent = "sharpmas (unconfigured; set CONTACT in .env)";

    /// <summary>Reads .env into the environment; already-set variables win.</summary>
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
            if (System.Environment.GetEnvironmentVariable(key) is null)
            {
                System.Environment.SetEnvironmentVariable(key, value);
            }
        }
    }

    /// <summary>Loads the file once, before anything first reads a variable.</summary>
    static Environment() => LoadEnvFile();

    /// <summary>The value of `key`, or null when it is unset or blank.</summary>
    /// <remarks>
    /// Blank counts as unset, so `CONTACT=` in the shipped template means what
    /// it looks like rather than producing an empty half of a user agent.
    /// Public so a client can read the variables only it needs, keeping the
    /// reach of a secret visible in the layout.
    /// </remarks>
    public static string? Get(string key)
    {
        var value = System.Environment.GetEnvironmentVariable(key)?.Trim();
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
        return (Get(RepoUrlKey), Get(ContactKey)) switch
        {
            (string repo, string contact) => $"{repo} by {contact}",
            (string repo, null) => $"{repo}",
            (null, string contact) => $"sharpmas by {contact}",
            (null, null) => UnconfiguredUserAgent,
        };
    }

    /// <summary>The session cookie, or null when it is unset or blank.</summary>
    /// <remarks>
    /// For callers that can work offline, where no cookie means skip the
    /// network rather than fail. Checking which session cached input came from
    /// needs the cookie but no requests.
    /// </remarks>
    public static string? CookieIfSet() => Get(CookieKey);

    /// <summary>The session cookie, required.</summary>
    /// <remarks>
    /// For callers that cannot proceed without one, such as building a client.
    /// The pair exists so the requirement is named here rather than at every
    /// call site, and so a run that needs no network never asks.
    /// </remarks>
    public static string Cookie()
    {
        return CookieIfSet()
            ?? throw new InvalidOperationException($"{CookieKey} is not set; add it to .env");
    }
}
