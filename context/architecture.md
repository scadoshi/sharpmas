# Architecture

The repo is ports and adapters.

- `Domain/` holds the puzzle types and imports nothing outside itself: no HTTP, no filesystem, no CLI.
- `Inbound/` is the way in and `Outbound/` is the way out.
- Both depend on the domain. The domain never depends on them.

```
sharpmas.slnx
src/Sharpmas/            class library, the whole tool
  Domain/                puzzles; knows nothing of HTTP, files, or the CLI
    Address/             which puzzle: Year, Day, Part
    Solution/            what a run produced: Answer, AnswerResult, Outcome,
                         Solved, Totals, the two verdicts, and ISolution
      Common/            helpers more than one day needs, empty to start
      YearTemplate/      copy this to start a year
  Inbound/               the CLI, and what each subcommand does
    Fetch/               downloading into the cache
    Solve/               running, validating, submitting, the day registry
  Outbound/              the world outside
    Client/              AocClient, SolverClient, Solver, and the environment they read
    Store/               the cache on disk
  Extensions/            small helpers on BCL types
src/Sharpmas.Cli/        entry point, one line
tests/Sharpmas.Tests/    xunit, mirrors the source tree
```

## How a solve runs

```
Cli               parses solve -y 2015 -d 1 --validate
  SolveRun        looks up the day in Solvers, gets Solver.Solve<Puzzle>
    Inputs        reads the cache, downloads whatever is missing
      Store       one directory per day of plain files
    Solver.Solve  T.Parse(input), times each part, catches each into AnswerResult
      Puzzle      the day's own logic
      SolverClient  checks each submittable answer when validating
    Outcome       renders answer, verdicts, and timing on one line
```

A day is a `Puzzle` class that implements `ISolution<Puzzle>`:

- A static `Parse` runs once and throws on bad input.
- `PartOne` and `PartTwo` read the parsed result.

The registry in `SolveRun` is the only list of what has been solved. A day missing from it gets skipped instead of failing the run.

## The cache

```
cache/2015/01/input.txt     the puzzle input, verbatim
cache/2015/01/session       hash of the cookie that fetched it
cache/2015/01/part_one.md   puzzle text
cache/2015/01/part_two.md   puzzle text, absent until part one is solved
```

Every file reads fine on its own.

- Inputs are account specific, so the session hash is what catches a swapped account.
- A missing `part_two.md` means part two is still locked. Each run checks again until it shows up.
