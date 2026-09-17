namespace Sharpmas.Outbound.Client;

/// <summary>A session cookie in the shape adventofcode.com accepts.</summary>
/// <remarks>
/// Parsed once at the edge so a mistyped value fails with its own message
/// rather than as a 400 partway through a run.
/// </remarks>
public sealed class SessionCookie
{
    /// <summary>How many hex characters the site's session cookie carries.</summary>
    /// <remarks>
    /// Observed rather than promised. The site reads this many and ignores
    /// anything after, so a longer value still authenticates while a shorter
    /// one does not, which makes a length check the only way to catch a
    /// truncated paste.
    /// </remarks>
    public const int Length = 128;

    public string Value { get; }

    SessionCookie(string value) => Value = value;

    /// <summary>Parses a raw value, throwing when it is not the right shape.</summary>
    public static SessionCookie Parse(string raw)
    {
        var value = raw.Trim();
        if (value.Length != Length)
        {
            throw new InvalidOperationException(
                $"COOKIE must be {Length} hex characters, got {value.Length}. "
                    + "Copy the whole session cookie value from your browser."
            );
        }
        if (!value.All(Uri.IsHexDigit))
        {
            throw new InvalidOperationException(
                "COOKIE must be hex characters only, and this one is not"
            );
        }
        return new SessionCookie(value);
    }

    public override string ToString() => Value;
}
