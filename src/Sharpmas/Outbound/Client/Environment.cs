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
    /// An exported variable beats the file, which is what makes a one-off
    /// override on the command line work. A missing file is ordinary, since
    /// <c>.env</c> is gitignored.
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
    /// Blank counts as unset, so <c>CONTACT=</c> in the shipped template means
    /// what it looks like. Public so a client reads only the variables it needs,
    /// keeping the reach of a secret visible.
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
    /// contact.
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
    /// network rather than fail.
    /// </remarks>
    public static string? CookieIfSet() => Get(CookieKey);

    /// <summary>The session cookie, required.</summary>
    /// <remarks>
    /// The pair exists so the requirement is named here rather than at every
    /// call site, and so a run that needs no network never asks.
    /// </remarks>
    public static string Cookie()
    {
        return CookieIfSet()
            ?? throw new InvalidOperationException($"{CookieKey} is not set; add it to .env");
    }
}
