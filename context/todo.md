# Todo

## Where this is

Translating rustmas file by file. `Domain/Address/` has `Year`, `Day`, and
`Part`. The approach is deliberate: the design is settled and recorded in
`rustmas/context/design/`, so this is transliteration plus asking what the C#
idiom is wherever the languages diverge.

Where rustmas already answered something, the notes below say so. Those are not
settled by authority, they are settled by having been tried, sometimes twice.
Reopen any of them if C# argues otherwise, but know what you are arguing with.

## Next

- **Fix the layering in rustmas first.** This blocks the translation rather than
  running alongside it.

  `domain/solutions/outcome.rs` imports `AocVerdict` and `SolverVerdict` from
  `outbound::client`, and `domain/solutions/solution.rs` imports `SolverClient`
  from the same place. Under ports and adapters the dependency only runs the
  other way, so the domain should own those types and the adapters should map
  their wire responses onto them.

  Both verdicts are already pure domain vocabulary. Neither mentions HTTP,
  neither carries a status code, and `Outcome` matches on them to render a line.
  They live in `outbound/` because that is where they were first written, not
  because anything put them there.

  `SolverClient` is the harder one, since `solve()` calls it. That is a port:
  the domain should own the interface and the adapter should implement it.

  Do not translate the current layering. Reorganise rustmas, then copy.

- **Then finish `Domain/Solutions/`.** Half built and not compiling, which is
  the intended stopping point:
  - `Answer.cs` has the three cases. Still needs the private constructor to
    close the hierarchy, `sealed` on the leaves, and the accessor that answers
    whether there is a submittable value.
  - `Outcome.cs` is a stub with public fields. Needs the four fields, the
    attaching methods that no-op unless the answer is submittable, and the
    rendering.
  - `SolverVerdict.cs` is the bare enum. Needs its rendering, and `AocVerdict`
    does not exist yet.
  - Then the `ISolution` interface.

- Lay out the library. The projects exist: `Sharpmas` holds everything,
  `Sharpmas.Cli` is the entry point, `Sharpmas.Tests` references the library.
  rustmas arranged its library as ports and adapters, a domain that knows nothing
  about HTTP or files, an inbound side for the CLI, an outbound side for the
  network and disk. That structure survived several refactors, so it is worth
  copying unless C# suggests otherwise.
- Decide how to locate `cache/`. rustmas reads the env var cargo sets, which has
  no C# equivalent: `AppContext.BaseDirectory` points at `bin/Debug/net10.0/`.
  Walking up to the `.slnx`, an env var, or the working directory are the
  options.
- Argument parsing. `System.CommandLine`, or something smaller. Needs `--year`
  and `--day` as filters, plus `--validate`, `--submit`, and a confirmation
  skip. Note the collision rustmas hit: `-y` is taken by `--year`, so the
  confirmation skip has no short flag.
- Decide how a day registers itself. Rust hand-wrote a registry because it has
  no runtime reflection. C# has it. Two things the registry must do besides
  dispatch: count how many parts a run would submit, and answer whether a day
  exists without holding its input, so an unwritten day is skipped before
  anything downloads.

## Soon

- The two HTTP clients. Contracts are already in
  `rustmas/context/references.md`, verified live and against the solver's
  source, so this is mostly transcription. Read it first.
- The cache. One directory per day of plain files: the input, a hash of the
  cookie that fetched it, and the puzzle text one file per part. rustmas tried
  a single structured document first and it read badly.
- How answers and verdicts are modelled. rustmas split them by provenance in the
  end: what the part computed, how long it took, and what each checker said are
  three different things from three different places. Verdicts are two types,
  since the solver and AOC can each say things the other cannot.
- Pull solutions across from `csharp-aoc`, which stays as it is.

## Later

- Timing per part, split from parse time. Measure before validating so no
  duration includes a network round trip.
- Refetch the day page after a correct part one, so part two's text lands
  without asking. Still unbuilt in rustmas too.
