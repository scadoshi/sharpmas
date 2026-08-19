using HtmlAgilityPack;
using Sharpmas.Domain.Address;
using Sharpmas.Domain.Solution;

namespace Sharpmas.Outbound.Client;

/// <summary>An authenticated handle to adventofcode.com.</summary>
/// <remarks>
/// Kept apart from <see cref="SolverClient"/> because the two differ in auth,
/// contract, and failure semantics. This one is authenticated and grades each
/// part exactly once.
/// </remarks>
public class AocClient
{
    /// <summary>Marks a puzzle part on a day page. Two means part two is unlocked.</summary>
    const string ArticleXPath = "//article[@class='day-desc']";

    static readonly Uri AocBaseUrl = new("https://adventofcode.com");

    /// <summary>The cookie this client authenticates with.</summary>
    /// <remarks>
    /// Kept as a value, not only as a header, because cached inputs record
    /// which session fetched them and that check needs something to compare.
    /// </remarks>
    public string Cookie { get; }

    public HttpClient Client { get; }

    /// <summary>Builds a client carrying the cookie and User-Agent on every request.</summary>
    /// <remarks>
    /// Both headers go on the client rather than each request, so nothing can
    /// send one without them. Only COOKIE is required, so a fresh clone runs
    /// without the rest.
    /// </remarks>
    public AocClient()
    {
        Cookie = Environment.Cookie();
        var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Cookie", $"session={Cookie}");
        // Without validation because the conventional contact string, a repo
        // URL followed by an address, is not a legal product token.
        client.DefaultRequestHeaders.TryAddWithoutValidation(
            "User-Agent",
            Environment.UserAgent()
        );
        Client = client;
    }

    /// <summary>Fetches a day's puzzle text, rendered from HTML.</summary>
    /// <remarks>
    /// The page holds one article per unlocked part, so counting them says which
    /// parts exist. Part two is null until part one is solved. Splitting on the
    /// markup rather than on prose means no phrase has to be matched and no flag
    /// can disagree with the text beside it.
    /// </remarks>
    public async Task<(string One, string? Two)> GetInstructions(Day day)
    {
        var url = new Uri(AocBaseUrl, $"{day.Year}/day/{day.Value}");
        var html = await GetString(url, $"puzzle text for year {day.Year} day {day.Value}");

        var document = new HtmlDocument();
        document.LoadHtml(html);
        var articles = document.DocumentNode.SelectNodes(ArticleXPath);

        if (articles is null || articles.Count == 0)
        {
            throw new InvalidOperationException(
                $"no puzzle text found for year {day.Year} day {day.Value}; "
                    + "the cookie may have expired, which redirects to the login page"
            );
        }

        var rendered = articles.Select(a => HtmlEntity.DeEntitize(a.InnerText).Trim()).ToList();
        return (rendered[0], rendered.Count > 1 ? rendered[1] : null);
    }

    /// <summary>Fetches a day's raw puzzle input, verbatim.</summary>
    /// <remarks>
    /// Not trimmed, since trailing whitespace is the site's to decide. A failure
    /// here usually means a bad cookie or a day that has not been released.
    /// </remarks>
    public async Task<string> GetInput(Day day)
    {
        var url = new Uri(AocBaseUrl, $"{day.Year}/day/{day.Value}/input");
        return await GetString(url, $"input for year {day.Year} day {day.Value}");
    }

    /// <summary>Submits an answer and reads the graded reply.</summary>
    /// <remarks>
    /// Everything comes back 200, so the verdict is entirely in the body. A part
    /// grades once: a second correct answer reports as already solved rather
    /// than confirming again.
    /// </remarks>
    public async Task<AocVerdict> SubmitAnswer(Day day, Part part, string answer)
    {
        var url = new Uri(AocBaseUrl, $"{day.Year}/day/{day.Value}/answer");
        var form = new FormUrlEncodedContent(
            [
                new KeyValuePair<string, string>("level", part.WireValue),
                new KeyValuePair<string, string>("answer", answer),
            ]
        );

        var response = await Client.PostAsync(url, form);
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();
        return VerdictFrom(body);
    }

    /// <summary>Fetches a URL as text, naming what was being fetched on failure.</summary>
    /// <remarks>
    /// A non-success status is a real failure here, unlike the solver, where a
    /// 400 carries the verdict. AOC answers 200 for everything it means to say.
    /// </remarks>
    async Task<string> GetString(Uri url, string what)
    {
        try
        {
            var response = await Client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception e) when (e is HttpRequestException or TaskCanceledException)
        {
            throw new InvalidOperationException($"failed to fetch {what}", e);
        }
    }

    /// <summary>Classifies AOC's HTML reply to a submission.</summary>
    /// <remarks>
    /// Direction is checked before the generic wrong-answer phrase, since a
    /// "too high" reply contains that phrase too. Strings verified live; see
    /// rustmas/context/references.md.
    /// </remarks>
    internal static AocVerdict VerdictFrom(string body)
    {
        if (body.Contains("That's the right answer"))
        {
            return new AocVerdict.Correct();
        }
        if (body.Contains("your answer is too high"))
        {
            return new AocVerdict.High();
        }
        if (body.Contains("your answer is too low"))
        {
            return new AocVerdict.Low();
        }
        if (body.Contains("You don't seem to be solving the right level"))
        {
            return new AocVerdict.AlreadySolved();
        }
        if (body.Contains("You gave an answer too recently"))
        {
            return new AocVerdict.Cooldown(WaitFrom(body));
        }
        return new AocVerdict.Incorrect();
    }

    /// <summary>Pulls the remaining wait out of a cooldown reply, such as 1m 0s.</summary>
    /// <remarks>
    /// Kept as the site phrased it, since it is only ever shown. Returns unknown
    /// rather than throwing, because a cooldown with an unreadable duration is
    /// still a cooldown.
    /// </remarks>
    internal static string WaitFrom(string body)
    {
        const string opening = "You have ";
        const string closing = " left to wait";

        var start = body.IndexOf(opening, StringComparison.Ordinal);
        if (start < 0)
        {
            return "unknown";
        }
        start += opening.Length;

        var end = body.IndexOf(closing, start, StringComparison.Ordinal);
        return end < 0 ? "unknown" : body[start..end].Trim();
    }
}
