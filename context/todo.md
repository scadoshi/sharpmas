# Todo

## Where this is

The tool is finished and works end to end, live. Two days are written: 2015 day
1 and 2016 day 1, all four answers confirmed by the solver and matching rustmas.

Solutions live on the `scadoshi` branch, per `rules/branches.md`. `main` is the
tool, an empty `Common/`, and the day template.

```
Domain/Address/      Year, Day, Part
Domain/Solution/     Answer, AnswerResult, AocVerdict, SolverVerdict,
                     Outcome, Solved, ISolution, Year2015/Day01
Inbound/             Cli, Inputs, Fetch/, Solve/ with the day registry
Outbound/Client/     Environment, AocClient, SolverClient, Solver.Solve
Outbound/Store/      Input, Instructions, Entry, Store
Extensions/          Causes on Exception, Formatted on TimeSpan
```

Builds clean with no warnings, and 49 tests pass. What is left is solutions, and
the helper types they will want.

The approach was deliberate: the design is settled and recorded in
`rustmas/context/design/`, so this was transliteration plus asking what the C#
idiom is wherever the languages diverge.

Where rustmas already answered something, the notes below say so. Those are not
settled by authority, they are settled by having been tried, sometimes twice.
Reopen any of them if C# argues otherwise, but know what you are arguing with.

## Next

- **Catch up to rustmas, which moved ahead on 2026-08-20. This is the next
  thing.** Its journal entry for that day has the full context. In rough order:

  - Renames in `Solved.cs` and its consumers: `Parse` to `ParsedIn`, `One`/`Two`
    to `PartOne`/`PartTwo`, `Total` to `TotalElapsed`.
  - The eager `Filter` type: `Filter.New(year, day)` validates up front with
    errors naming the value and the live bound (`day 13 is outside 1..=12` for
    2025), and expansion becomes infallible. Kills the silent exit on
    `-y 2030`. `Day.Each` becomes `Day.Matching(filter)` over an infallible
    `Day.All()`. The C# wrinkle: sharpmas's `Day.Each` never had a Result to
    delete, so the win here is only the eager errors, which is still the point.
  - `OutOfRange`-style errors split per producer: `Year`'s constructor exception
    should name the value and range, `Day`'s the year's own day count.
  - `HasSecondPuzzle` on `Day`: day 25's second star is awarded rather than
    puzzled, so nothing should keep fetching its part two.
  - The run-fold question: rustmas moved each command's `run` into its module.
    sharpmas's `SolveRun`/`FetchRun` classes are the C# equivalent already, so
    likely nothing to do, but check the shape reads the same.

  Added 2026-08-21, from the LazyAocClient session:

  - Verdicts named by provenance on `Outcome`: `Verdict`/`WithVerdict` become
    `SolverVerdict`/`WithSolverVerdict`, `Submission`/`WithSubmission` become
    `AocVerdict`/`WithAocVerdict`. One word each was too easy to confuse.
  - `LazyAocClient` moves from `Inbound/` to beside `AocClient` in
    `Outbound/Client/`: it holds only client knowledge (build on first use,
    from the environment), so it lives with the thing it wraps. rustmas put it
    there after making the layering argument properly.
  - The eager path reads `default` then `Connected()` when submitting, rather
    than a second constructor; rustmas grew `from_env` for it but the call-site
    override is the shape both repos should read as.
  - `solved.parsed_in` printing and the `has_second_puzzle` gate ride along with
    items already listed above.

- **Add `Answer.Unwritten` while in there.** Still outstanding from the original
  list: `None` currently covers both "no answer exists" and "nobody wrote this
  part", which is the ambiguous-absence pattern. `Unwritten` splits the second
  off, prints `(unwritten)`, and the `YearTemplate` stubs switch to returning it
  so a stub cannot read as a finished part with nothing to say. rustmas has it;
  `Answer.cs` has `Value`, `Visual`, and `None` only.

- **The rest of `Common/`. Scott is porting these himself**, so leave them alone
  unless asked. `Cell` is the one left from rustmas, counting rows down from the
  top where `Point` counts y upward. Nothing is blocked on it, but the first grid
  day will want it.

  The pattern the finished three set: payload-free sets are enums with an
  extension class rather than record hierarchies, positions are
  `readonly record struct` for free equality and no allocation per step, and
  parsing follows the BCL convention where `Parse` throws and `TryParse` returns
  a bool. Tests mirror rustmas unless C# genuinely needs more, which so far has
  meant guarding what Rust's compiler guarantees and C#'s does not.

- **Close the hierarchies.** `Answer`, `AocVerdict`, and `SolverVerdict` are
  abstract records with sealed leaves, but nothing stops outside code adding a
  case. A `private Answer() { }` on the base fixes that, since only nested types
  can reach it. Worth doing once the shapes stop moving.

- **Test the `.env` parsing, but only when it next changes.** Deliberately left
  alone. It is an error path nothing else exercises, which normally earns a test,
  but a broken parse shows up on the very next run as the unconfigured user agent
  or a missing cookie, where a bad cache read fails silently and much later. If
  that parser is touched again, give `LoadEnvFile` a path parameter first, the
  way `Store` takes a root, and the tests become as easy as `Store`'s.

## Soon

- Solutions. `csharp-aoc` has some worth pulling across, and rustmas has day one
  of every year except 2019 to compare against.
- Swap the hand-rolled argument parser for `System.CommandLine` if the surface
  grows past two subcommands and five flags. Hand-rolled today because that was
  about sixty lines with no dependency and the package's API has churned across
  previews.

## Later

- Nothing outstanding here. Both open questions closed on 2026-08-18: the cookie
  readers live on `Environment` in both repos, and the day template exists.

