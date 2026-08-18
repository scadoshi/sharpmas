# sharpmas

Advent of Code tooling in C#. Downloads your puzzle inputs, runs your solutions,
checks the answers against an independent solver, and submits them for stars.

A rebuild of [rustmas](https://github.com/scadoshi/rustmas) in a language with
different defaults.

No solutions ship with it. Clone it, add your `.env`, and write your first day.
scadoshi's own solutions live on the `scadoshi` branch if you want worked
examples.

## Setup

You need your Advent of Code session cookie. Log in at
[adventofcode.com](https://adventofcode.com), open your browser dev tools, and
copy the value of the cookie named `session`.

Copy [`.env.template`](.env.template) to `.env` and fill it in:

```
COOKIE=<your session cookie>
CONTACT=<an address AOC can reach you at>
REPO_URL=<your fork, if you forked>
```

Only `COOKIE` is required, and it belongs to your account, so `.env` is
gitignored. `CONTACT` and `REPO_URL` shape the `User-Agent`, because the site
asks automated clients to be reachable. Neither has a default that names anyone,
so leaving them blank identifies the tool and nobody else.

`.env` is read from the repo root, found by walking up from the running assembly
until the `.slnx` turns up. An already-exported variable beats the file, so a
one-off override on the command line works.

## Running it

One binary, one subcommand per mode.

```
dotnet run --project src/Sharpmas.Cli -- solve -y 2015 -d 1
dotnet run --project src/Sharpmas.Cli -- fetch -y 2015 -d 1
```

The `--` is required: without it `dotnet run` reads the flags as its own.

Worth an alias while you are working on it:

```sh
alias sharpmas='dotnet run --project src/Sharpmas.Cli --'
sharpmas solve -y 2015 -d 1
```

Debug builds are slow enough to notice on anything that brute forces, so
`--configuration Release` is worth reaching for once a day takes longer than you
want to sit through. It goes before the `--`:

```
dotnet run --project src/Sharpmas.Cli --configuration Release -- solve -y 2015 -d 1
```

## fetch

Downloads puzzle inputs and instructions into `cache/<year>/<NN>/`.

```
sharpmas fetch                 # everything
sharpmas fetch -y 2015         # one year
sharpmas fetch -d 1            # day 1 of every year
sharpmas fetch -y 2015 -d 1    # one puzzle
```

| Flag | Meaning |
| --- | --- |
| `-y`, `--year` | Only this year. Omit for all. |
| `-d`, `--day` | Only this day. Omit for all. |

Both flags are filters rather than a lookup, so omitting one means all of them.

Re-running is safe. Inputs never change, so a cached one is never fetched again;
Advent of Code asks that you not re-download. Instructions are different, since
part two stays locked until part one is solved. A day cached without
`part_two.md` is incomplete rather than finished, so `fetch` asks for it again on
every run until it arrives.

## solve

Runs your solutions, with the same filters.

```
sharpmas solve -y 2015 -d 1              # offline
sharpmas solve -y 2015 -d 1 --validate   # check the answers
sharpmas solve -y 2015 -d 1 --submit     # check, then send for stars
```

| Flag | Meaning |
| --- | --- |
| `-y`, `--year` | Only this year. Omit for all. |
| `-d`, `--day` | Only this day. Omit for all. |
| `-v`, `--validate` | Check each answer against a third-party solver, one request per part. |
| `-s`, `--submit` | Submit to Advent of Code. Implies `--validate`. |
| `--yes` | Skip the confirmation prompt on an unfiltered `--submit`. |

Solving reads inputs from disk and downloads what is missing. With no cookie set
it stays entirely offline. With one set, a day still waiting on part two costs a
request to see whether it has unlocked. `--validate` needs no cookie either,
since the third-party solver has no accounts.

`--submit` always validates first and only sends what the solver agrees with,
because a wrong answer to Advent of Code earns a cooldown that escalates with
repeats. If the solver has no implementation for that puzzle, which happens
during a live event, the answer is submitted anyway and flagged as unchecked.

A new star on part one unlocks part two, so `--submit` fetches its text before
the run finishes rather than leaving it for next time.

Run `--submit` with no year or day and it would post every solved part, so it
prints the count and asks first. `--yes` skips that. There is no short flag for
it on purpose, since `-y` is `--year` and this one is worth typing out.

### Reading the output

```
year 2015 day 1 in 950.7µs (35.5µs parsing)
  part one: 138 (correct) [530.9µs]
  part two: 1771 (correct) [384.3µs]
```

Until a day has a solution, `solve` skips it. Asking for one by name says so
rather than printing nothing.

Each part is one line: the answer, then whatever is known about it, then how long
it took. Timings cover parsing and solving only, never the network.

| Note | Meaning |
| --- | --- |
| nothing | Solved offline, unchecked |
| `correct` | The solver agrees |
| `high`, `low`, `incorrect` | The solver disagrees, so nothing was submitted |
| `new star` | Advent of Code just accepted it |
| `starred` | Advent of Code says the part was already solved |
| `unsupported` | The solver has no implementation for this puzzle |
| `rate limited, 1m 0s left to wait` | Advent of Code refused to grade, wait it out |
| `(none)` | The part has no answer, such as day 25 part two |
| `error: ...` | The part failed. The other part still ran |

Advent of Code grades each part exactly once, so a part solved earlier reports
`starred` rather than confirming the answer again.

## Adding a solution

Three steps. Say you are writing 2015 day 2.

Copy the template, which is compiled on every build and so cannot drift from the
interface:

```sh
cp -r src/Sharpmas/Domain/Solution/YearTemplate \
      src/Sharpmas/Domain/Solution/Year2015
```

That gives you `Year2015/Day01/Puzzle.cs` with both parts stubbed. Rename the
folder to the day you want, and change the namespace to match the path:

```csharp
namespace Sharpmas.Domain.Solution.Year2015.Day02;
```

Write the parts:

```csharp
public Answer PartOne() => Answer.Solved(Input.Length.ToString());
```

Then register it, in `Solvers` in `src/Sharpmas/Inbound/Solve/SolveRun.cs`:

```csharp
[(2015, 2)] = Outbound.Client.Solver.Solve<Domain.Solution.Year2015.Day02.Puzzle>,
```

That dictionary is the only list of what has been solved. A day missing from it
is skipped rather than failing.

Anything more than one day needs goes in `src/Sharpmas/Domain/Solution/Common/`,
which ships empty. Grid and geometry work is what usually ends up there, since
Advent of Code returns to it every year.

Write those the second day that wants them rather than the first, and give them
tests: a break in a shared type corrupts every day at once, where a single day's
logic is already checked by `--validate`.

Every day's type is named `Puzzle`, with the namespace carrying the coordinate,
so two years never collide. The registry names them fully qualified for the same
reason.

`Parse` runs once so both parts read the result, and throws if the input will
not parse, which ends that day without touching the others. Parts return
`Answer.Solved(value)` for something submittable, `Answer.Visual(art)` for a grid
you read yourself, and `Answer.None()` when there is genuinely no answer, such as
day 25 part two. A part that throws is caught and reported on its own line, so
the other part still runs.

Returning art rather than printing it keeps solving free of IO.

## Layout

```
src/
  Sharpmas.Cli/              entry point, one line
  Sharpmas/
    Domain/                  puzzles, with no idea HTTP or files exist
      Address/               which puzzle: Year, Day, Part
      Solution/              what a puzzle produced: Answer, Outcome, ISolution
        Common/              helpers more than one day needs, empty to start
        YearTemplate/Day01/  copy this to start a year
        Year<year>/Day<NN>/  your days go here, one folder each
    Inbound/                 the CLI, and what each subcommand does
      Fetch/                 downloading
      Solve/                 running, validating, submitting, the day registry
    Outbound/                the world outside
      Client/                the two services, and the environment they read
      Store/                 the cache on disk
    Extensions/              small helpers on BCL types
tests/Sharpmas.Tests/        mirrors the source tree
```

The domain knows nothing about HTTP, the filesystem, or the command line, which
is what keeps a puzzle testable without any of them.

## Start here

[`context/`](context/) covers what this is, how to work on it, and which rustmas
decisions were settled for good reasons versus which ones C# gets to reopen.

## License

MIT. See [LICENSE](LICENSE).
