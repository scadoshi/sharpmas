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

- **Finish `IntExtensions`. This is the next thing.** A stub is sitting
  uncommitted at `Domain/Solution/Common/IntExtensions.cs`. The plan:
  `CheckedAdd(int rhs)` returning `int?`, widen to `long` and range-check, plus
  `CheckedSub` as its own method since `CheckedAdd(-rhs)` breaks when `rhs` is
  `int.MinValue`. The stub needs a namespace, loses the stray semicolon after
  the extension block, and `CheckedValue` becomes `CheckedAdd`. Tests as
  theories at `tests/.../Common/IntExtensionsTests.cs`; the case that proves
  `CheckedSub` earns its keep is `0.CheckedSub(int.MinValue)`, which is null
  because the true result is one past `int.MaxValue`.

  `Cell` cannot use these, since it needs the `uint` range; its private helper
  stays. The consumers are `Point.CheckedMoved`, if built, and days doing raw
  arithmetic.

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

- **Add `Answer.Unwritten`.** rustmas grew a fourth case after `None` turned out
  to mean three different things: no answer exists, nobody has written this part
  yet, and there is no such puzzle. `Unwritten` splits the second off and prints
  `(unwritten)`. `Answer.cs` currently has `Value`, `Visual`, and `None` only.

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

