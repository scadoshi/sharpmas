# sharpmas

Advent of Code tooling in C#. Downloads your puzzle inputs, runs your solutions,
checks the answers against an independent solver, and submits them for stars.

This is Scotty's working branch, with his solutions attached. The `main` branch
is the same tool with no solutions, which is the one to clone if you want a
starting point.

## Quick start

**1. Add your session cookie.** Log in at
[adventofcode.com](https://adventofcode.com), copy the value of the cookie named
`session` from your browser's dev tools, then:

```sh
cp .env.template .env    # paste the cookie into COOKIE=
```

Only `COOKIE` is required. `CONTACT` and `REPO_URL` shape the `User-Agent`,
since the site asks automated clients to be reachable.

**2. Run something.** One binary, one subcommand per mode, and the `--` matters:
without it `dotnet run` eats the flags as its own.

```sh
alias sharpmas='dotnet run --project src/Sharpmas.Cli --'

sharpmas fetch -y 2015 -d 1              # download one puzzle into cache/
sharpmas solve -y 2015 -d 1              # run your solution offline
sharpmas solve -y 2015 -d 1 --validate   # check answers against the solver
sharpmas solve -y 2015 -d 1 --submit     # check, then send for stars
```

`-y` and `-d` are filters: omit either for all of them. `--submit` validates
first and only sends what the solver agrees with, because wrong answers cost an
escalating cooldown. Unfiltered submits ask before posting everything; `--yes`
skips that.

**3. Write a day.** Say 2015 day 1. Copy the template, which is compiled on
every build and so cannot drift:

```sh
cp -r src/Sharpmas/Domain/Solution/YearTemplate src/Sharpmas/Domain/Solution/Year2015
```

Fix the namespace in the copied `Day01/Puzzle.cs` to match the path:

```csharp
namespace Sharpmas.Domain.Solution.Year2015.Day01;
```

Write the parts. `Parse` runs once and throws on bad input; each part returns
`Answer.Solved(value)`, `Answer.Visual(art)`, or `Answer.None()`:

```csharp
public Answer PartOne() => Answer.Solved(Input.Length.ToString());
```

Register it in `Solvers` in `src/Sharpmas/Inbound/Solve/SolveRun.cs`:

```csharp
[(2015, 1)] = Outbound.Client.Solver.Solve<Domain.Solution.Year2015.Day01.Puzzle>,
```

That dictionary is the only list of what has been solved. Then:

```sh
sharpmas solve -y 2015 -d 1 --validate
```

## Reading the output

```
year 2015 day 1 in 950.7µs (35.5µs parsing)
  part one: 138 (correct) [530.9µs]
  part two: 1771 (correct) [384.3µs]
```

One line per part: the answer, what is known about it, and how long it took.
Timings never include the network.

| Note | Meaning |
| --- | --- |
| nothing | Solved offline, unchecked |
| `correct` | The solver agrees |
| `high`, `low`, `incorrect` | The solver disagrees, so nothing was submitted |
| `new star` | Advent of Code just accepted it |
| `starred` | Advent of Code says the part was already solved |
| `unsupported` | The solver has no implementation for this puzzle |
| `rate limited, 1m 0s left to wait` | Advent of Code refused to grade |
| `(none)` | The part has no answer, such as day 25 part two |
| `error: ...` | The part failed. The other part still ran |

## Worth knowing

- Re-running `fetch` is safe: inputs are never re-downloaded, and a day still
  waiting on part two is rechecked until it unlocks.
- With no cookie set, `solve` works entirely offline from the cache, and
  `--validate` still works, since the solver needs no account.
- Shared helpers more than one day needs go in `Domain/Solution/Common/`.
- Debug builds are slow on brute-force days;
  `dotnet run --project src/Sharpmas.Cli -c Release -- solve ...` when it drags.

## Going deeper

- [`context/README.md`](context/README.md) covers the architecture, the layout,
  the two service contracts, and every design decision in brief.
- [`context/rules/`](context/rules/) holds the working rules: commit
  guidelines, doc comment style, and the branch model.
- [rustmas](https://github.com/scadoshi/rustmas) is the original this rebuilds;
  its `context/design/` and `references.md` carry the full reasoning and the
  service contracts as verified.

## License

MIT. See [LICENSE](LICENSE).
