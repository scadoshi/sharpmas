using Sharpmas.Domain.Address;
using Sharpmas.Outbound.Client;

namespace Sharpmas.Inbound.Fetch;

/// <summary>Downloads puzzle inputs and instructions into the cache.</summary>
public static class FetchRun
{
    /// <summary>Fills the cache for every day the filters allow.</summary>
    /// <remarks>
    /// Files on disk are left alone, and one failed download stops the rest,
    /// since a failure is usually a bad cookie and retrying would just repeat
    /// it against the site.
    /// </remarks>
    public static async Task Run(FetchArgs args)
    {
        // Built on first download, so a fully cached run needs no cookie.
        var client = new LazyAocClient();

        foreach (var day in Day.Each(args.Year, args.Day))
        {
            await Inputs.EnsureEntry(client, day);
        }
    }
}
