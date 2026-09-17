using Sharpmas.Domain.Address;
using Sharpmas.Outbound.Client;
using Sharpmas.Outbound.Store;
// Aliased because System.Environment is implicitly imported and collides.
using Environment = Sharpmas.Outbound.Client.Environment;

namespace Sharpmas.Inbound;

/// <summary>Getting a day's input and puzzle text, from cache or from the site.</summary>
public static class Inputs
{
    /// <summary>Returns a day's cached entry, downloading whatever is missing.</summary>
    /// <remarks>
    /// A missing part two means incomplete, so it is rechecked every run until
    /// part one is solved. An input from another session is refetched.
    /// </remarks>
    public static async Task<Entry> EnsureEntry(LazyAocClient client, Day day)
    {
        // Absent when no cookie is configured, leaving a cached entry usable.
        var cookie = Environment.CookieIfSet();
        var cached = Store.ReadEntry(day);

        if (cached is null)
        {
            var fresh = new Entry
            {
                Input = await FetchInput(client, day, cookie),
                Instructions = await FetchInstructions(client, day),
            };
            Store.WriteEntry(day, fresh);
            Console.WriteLine($"fetched year {day.Year} day {day.Value}");
            return fresh;
        }

        var staleSession = cookie is not null && !cached.Input.IsFrom(cookie);
        // No cookie means nothing to ask with, so an incomplete cache stays.
        var chasePartTwo =
            cached.Instructions.PartTwo is null && day.HasSecondPuzzle && cookie is not null;

        if (!staleSession && !chasePartTwo)
        {
            return cached;
        }

        var input = cached.Input;
        if (staleSession)
        {
            input = await FetchInput(client, day, cookie);
            Console.WriteLine($"refetched input for year {day.Year} day {day.Value}");
        }

        var instructions = cached.Instructions;
        if (chasePartTwo)
        {
            instructions = await FetchInstructions(client, day);
            if (instructions.PartTwo is not null)
            {
                Console.WriteLine($"part two unlocked for year {day.Year} day {day.Value}");
            }
        }

        var entry = new Entry { Input = input, Instructions = instructions };
        Store.WriteEntry(day, entry);
        return entry;
    }

    /// <summary>Downloads a day's input and tags it with the session that got it.</summary>
    static async Task<Input> FetchInput(LazyAocClient client, Day day, SessionCookie? cookie)
    {
        var data = await client.Connected().GetInput(day);
        return Input.Fetched(cookie ?? Environment.Cookie(), data);
    }

    /// <summary>Downloads a day's puzzle text, both parts if both are unlocked.</summary>
    static async Task<Instructions> FetchInstructions(LazyAocClient client, Day day)
    {
        var (one, two) = await client.Connected().GetInstructions(day);
        return new Instructions { PartOne = one, PartTwo = two };
    }
}
