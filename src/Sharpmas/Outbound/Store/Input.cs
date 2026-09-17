using System.Security.Cryptography;
using System.Text;
using Sharpmas.Outbound.Client;

namespace Sharpmas.Outbound.Store;

/// <summary>A day's cached input, and the session that fetched it.</summary>
/// <remarks>
/// Inputs are account specific, so the same file answers differently for two
/// people. Carrying a digest of the cookie is what catches a swapped account,
/// which nothing else would.
/// </remarks>
public sealed class Input
{
    /// <summary>A digest of the cookie that fetched this input.</summary>
    /// <remarks>The cookie itself never reaches disk.</remarks>
    public string Hash { get; }

    /// <summary>The puzzle input, verbatim.</summary>
    public string Data { get; }

    Input(string hash, string data)
    {
        Hash = hash;
        Data = data;
    }

    /// <summary>A freshly downloaded input, tagged with the session that got it.</summary>
    public static Input Fetched(SessionCookie cookie, string data) =>
        new(HashCookie(cookie), data);

    /// <summary>Rebuilt from disk, where the hash was already generated.</summary>
    public static Input FromParts(string hash, string data) => new(hash, data);

    /// <summary>Whether this input was fetched with the given cookie.</summary>
    public bool IsFrom(SessionCookie cookie) => Hash == HashCookie(cookie);

    static string HashCookie(SessionCookie cookie)
    {
        var digest = SHA256.HashData(Encoding.UTF8.GetBytes(cookie.Value));
        return Convert.ToHexString(digest).ToLowerInvariant();
    }
}
