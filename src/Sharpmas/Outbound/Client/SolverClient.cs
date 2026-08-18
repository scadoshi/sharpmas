namespace Sharpmas.Outbound.Client;

using System.Text;
using Sharpmas.Domain.Address;
using Sharpmas.Domain.Solution;

public class SolverClient
{
    static readonly Uri[] BaseUrls =
    [
        new("https://advent.fly.dev"),
        new("https://aoc.fornwall.workers.dev"),
        new("https://mystifying-blackwell-9e705f.netlify.app"),
    ];

    const string RepoUrlKey = "REPO_URL";
    const string UnconfiguredUserAgent = "sharpmas (unconfigured; set CONTACT in .env)";

    public string UserAgent { get; }
    public HttpClient Client { get; }

    public SolverClient()
    {
        UserAgent = Env.UserAgent();
        HttpClient client = new();
        client.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent);
        Client = client;
    }

    public async Task<SolverVerdict> ValidateAnswer(Day day, string input, Part part, string answer)
    {
        foreach (Uri baseUrl in BaseUrls)
        {
            var fullUrl = new Uri(baseUrl, $"solve/{day.Year}/{day.Value}/{part.WireValue}");

            var content = new StringContent(input, Encoding.UTF8, "text/plain");
            var response = await Client.PostAsync(fullUrl, content);
            var body = (await response.Content.ReadAsStringAsync()).Trim();

            if (
                response.IsSuccessStatusCode
                && long.TryParse(answer, out long answerLong)
                && long.TryParse(body, out long bodyLong)
            )
            {
                return SolverVerdict.From(answerLong.CompareTo(bodyLong));
            }

            if (response.IsSuccessStatusCode)
            {
                return SolverVerdict.From(answer == body);
            }

            bool isClientError = (int)response.StatusCode is >= 400 and < 500;
            if (isClientError)
            {
                if (body.StartsWith("Unsupported"))
                {
                    return new SolverVerdict.Unsupported();
                }
                throw new InvalidOperationException(
                    $"solver at {fullUrl} rejected the request: {body}"
                );
            }

            Console.Error.WriteLine(
                $"solver at {fullUrl} returned {response.StatusCode}: {body}; trying next url"
            );
        }
        throw new InvalidOperationException(
            $"failed to check answser for year: {day.Year} and day {day.Value} via all urls"
        );
    }
}
