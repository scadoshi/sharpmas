using System.Security.Cryptography;
using System.Text;

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
    public static Input Fetched(string cookie, string data) => new(HashCookie(cookie), data);

    /// <summary>Rebuilt from disk, where the hash was already generated.</summary>
    public static Input FromParts(string hash, string data) => new(hash, data);

    /// <summary>Whether this input was fetched with the given cookie.</summary>
    public bool IsFrom(string cookie) => Hash == HashCookie(cookie);

    static string HashCookie(string cookie)
    {
        var digest = SHA256.HashData(Encoding.UTF8.GetBytes(cookie));
        return Convert.ToHexString(digest).ToLowerInvariant();
    }
}
