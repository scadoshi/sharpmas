# Todo

## Where this is

The tool is finished and works end to end, live: `solve -y 2015 -d 1 --validate`
fetches from the site, writes the cache, solves, and checks both answers against
the third-party solver. Answers match rustmas.

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

- **Give solutions their own branch. This is the next thing.** `branches.md` in
  rustmas has the rule and both repos are meant to follow it: `main` is the tool,
  a personal branch adds solutions, and changes flow one way by merging `main`
  down. sharpmas has only `main`, and now has a day and a registry entry sitting
  on it. Sort this before there are many.

- **Port the solution helper types.** `Point`, `Cell`, `Direction`, `Turn`, and
  their tests are the whole remaining gap against rustmas's test count, and days
  will want them immediately. They are pure domain, so nothing about them is
  blocked.

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

