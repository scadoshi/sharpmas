# Architecture

Ports and adapters. `Domain/` holds the puzzle types and imports nothing outside
itself: no HTTP, no filesystem, no CLI. `Inbound/` is the way in, `Outbound/`
is the way out, and each depends on the domain rather than the reverse.

```
sharpmas.slnx
src/Sharpmas/            class library, the whole tool
  Domain/                puzzles; knows nothing of HTTP, files, or the CLI
    Address/             which puzzle: Year, Day, Part
    Solution/            what a run produced: Answer, AnswerResult, Outcome,
                         Solved, the two verdicts, and ISolution
      Common/            helpers more than one day needs, empty to start
      YearTemplate/      copy this to start a year
  Inbound/               the CLI, and what each subcommand does
    Fetch/               downloading into the cache
    Solve/               running, validating, submitting, the day registry
  Outbound/              the world outside
    Client/              AocClient, SolverClient, and the environment they read
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

A day is a `Puzzle` class implementing `ISolution<Puzzle>`: a static `Parse`
that runs once and throws on bad input, and `PartOne`/`PartTwo` reading the
parsed result. The registry in `SolveRun` is the only list of what has been
solved; a day missing from it is skipped rather than failing.

## The cache

```
cache/2015/01/input.txt     the puzzle input, verbatim
cache/2015/01/session       hash of the cookie that fetched it
cache/2015/01/part_one.md   puzzle text
cache/2015/01/part_two.md   puzzle text, absent until part one is solved
```

Every file is readable on its own. The session hash is what catches a swapped
account, since inputs are account specific. Part two's absence means still
locked, which is rechecked each run until it arrives.
