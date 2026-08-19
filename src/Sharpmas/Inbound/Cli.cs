using Sharpmas.Inbound.Fetch;
using Sharpmas.Inbound.Solve;

namespace Sharpmas.Inbound;

/// <summary>The command line. One binary, one subcommand per mode.</summary>
/// <remarks>
/// Hand-parsed rather than pulled from a package, since the surface is two
/// subcommands and five flags. Swap in System.CommandLine if it grows.
/// </remarks>
public static class Cli
{
    const string Usage = """
        sharpmas <command> [options]

        Commands:
          fetch    Download puzzle inputs and text into the cache
          solve    Run solutions, optionally validating and submitting them

        Options:
          -y, --year <n>   Only this year (omit for all)
          -d, --day <n>    Only this day (omit for all)
          -v, --validate   Check answers against the third-party solver (solve only)
          -s, --submit     Submit answers to adventofcode.com (solve only)
              --yes        Skip the confirmation prompt when submitting unfiltered
          -h, --help       Print this
        """;

    /// <summary>Parses the arguments and runs whichever subcommand was given.</summary>
    public static async Task<int> Run(string[] args)
    {
        if (args.Length == 0 || args[0] is "-h" or "--help")
        {
            Console.WriteLine(Usage);
            return args.Length == 0 ? 1 : 0;
        }

        var command = args[0];
        var rest = args[1..];

        try
        {
            var parsed = Parse(rest);
            switch (command)
            {
                case "fetch":
                    await FetchRun.Run(new FetchArgs { Year = parsed.Year, Day = parsed.Day });
                    return 0;
                case "solve":
                    await SolveRun.Run(
                        new SolveArgs
                        {
                            Year = parsed.Year,
                            Day = parsed.Day,
                            Validate = parsed.Validate,
                            Submit = parsed.Submit,
                            Yes = parsed.Yes,
                        }
                    );
                    return 0;
                default:
                    Console.Error.WriteLine($"unknown command: {command}");
                    Console.Error.WriteLine(Usage);
                    return 1;
            }
        }
        catch (Exception e)
        {
            // The whole chain, since the outermost message rarely names the day.
            Console.Error.WriteLine($"Error: {string.Join(": ", Causes(e))}");
            return 1;
        }
    }

    static IEnumerable<string> Causes(Exception e)
    {
        for (Exception? current = e; current is not null; current = current.InnerException)
        {
            yield return current.Message;
        }
    }

    /// <summary>Every flag a subcommand might take, whether or not it uses them.</summary>
    internal record Options(int? Year, int? Day, bool Validate, bool Submit, bool Yes);

    internal static Options Parse(string[] args)
    {
        int? year = null;
        int? day = null;
        var validate = false;
        var submit = false;
        var yes = false;

        for (var i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "-y" or "--year":
                    year = ParseValue(args, ref i, "year");
                    break;
                case "-d" or "--day":
                    day = ParseValue(args, ref i, "day");
                    break;
                case "-v" or "--validate":
                    validate = true;
                    break;
                case "-s" or "--submit":
                    submit = true;
                    break;
                case "--yes":
                    yes = true;
                    break;
                default:
                    throw new ArgumentException($"unknown option: {args[i]}");
            }
        }

        return new Options(year, day, validate, submit, yes);
    }

    /// <summary>Reads the number after a flag, failing rather than defaulting.</summary>
    static int ParseValue(string[] args, ref int i, string name)
    {
        if (i + 1 >= args.Length)
        {
            throw new ArgumentException($"--{name} needs a value");
        }
        i++;
        if (!int.TryParse(args[i], out var value))
        {
            throw new ArgumentException($"--{name} must be a number, got {args[i]}");
        }
        return value;
    }
}
