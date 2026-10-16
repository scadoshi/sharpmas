# Todo

## Where this is

The tool is finished and works end to end, live. 134 tests pass and the build is clean with no warnings.

It matches rustmas feature for feature. The catch-up that had been queued since 2026-08-20 closed on 2026-08-23, written on the flight to London, and the run summary ported across on 2026-09-13.

Two days are written, 2015 day 1 and 2016 day 1. All four answers are confirmed by the solver and match rustmas.

`Common/` is fully ported. `Cell`, `Direction`, `Point`, and `Turn` come from rustmas. `IntExtensions` has no rustmas counterpart, because Rust's checked arithmetic is already in the standard library.

Solutions live on the `scadoshi` branch, per `rules/branches.md`. `main` has the tool, an empty `Common/`, and the day template.

```
Domain/Address/      Year, Day, Part, Filter
Domain/Solution/     Answer, AnswerResult, AocVerdict, SolverVerdict,
                     Outcome, Solved, Totals, ISolution, Common/,
                     Year2015/Day01, Year2016/Day01, Year2016/Pose
Inbound/             Cli, Inputs, Fetch/, Solve/ with the day registry
Outbound/            Paths
Outbound/Client/     Environment, SessionCookie, AocClient, LazyAocClient,
                     SolverClient, Solver.Solve
Outbound/Store/      Input, Instructions, Entry, Store
Extensions/          Causes on Exception, Formatted on TimeSpan
```

What's left is solutions.

The approach was deliberate. The design is settled and recorded in `rustmas/context/design/`, so this was transliteration, plus asking what the C# idiom is wherever the languages diverge.

Where rustmas already answered something, it was settled by having been tried, sometimes twice. Reopen any of it if C# argues otherwise, but know what you're arguing with.

## Closed on 2026-08-23

Detail is in [`progress/journal/2026-08-23.md`](progress/journal/2026-08-23.md). These are listed here only because this file had carried them as pending since the 20th.

- The eager `Filter`, with `Day.All()` and `Day.Matching(filter)` replacing `Day.Each`. Guard messages stayed in the native dialect, since the framework helpers already name the value and the live bound.
- `Solved`'s renames, and the verdicts named by provenance on `Outcome`.
- `HasSecondPuzzle` wired into `EnsureEntry`, so a finished day 25 stops costing a request every run.
- `Answer.Unwritten`, in the template stubs too.
- All four hierarchies closed with private base constructors.
- `LazyAocClient` moved beside `AocClient`.
- The `SolveRun`/`FetchRun` shape check against rustmas's run-fold, which came back empty as predicted.

## Closed on 2026-09-13

Detail is in [`progress/journal/2026-09-13.md`](progress/journal/2026-09-13.md).

- `Totals`, `Outcome.SolveTime`, and `Part.Word`, wired into `SolveRun` and printed only when more than one day ran.
- A comment pass across the repo, 42 lines shorter.

Both are tool changes sitting on `scadoshi`. They still need moving to `main` and merging down, per `rules/branches.md`.

## Next

Solutions. `csharp-aoc` has some worth pulling across, and rustmas has day one of every year except 2019 to compare against.

## Soon

Swap the hand-rolled argument parser for `System.CommandLine` if the surface grows past two subcommands and five flags. It's hand-rolled today because that came to about sixty lines with no dependency, and the package's API has churned across previews.

## Deliberately deferred

### New helpers in `Common/`, when a second day wants one

Nothing is outstanding, so this is the pattern the finished five set rather than a task:

- Payload-free sets are enums with an extension class rather than record hierarchies.
- Positions are `readonly record struct`, for free equality and no allocation per step.
- Parsing follows the BCL convention, where `Parse` throws and `TryParse` returns a bool.
- Tests mirror rustmas unless C# genuinely needs more. So far that has meant guarding what Rust's compiler guarantees and C#'s does not.

### Test the `.env` parsing, but only when it next changes

It's an error path nothing else exercises, which normally earns a test. But a broken parse shows up on the very next run, as the unconfigured user agent or a missing cookie, where a bad cache read fails silently and much later.

If that parser is touched again, give `LoadEnvFile` a path parameter first, the way `Store` takes a root. Then the tests become as easy as `Store`'s.
