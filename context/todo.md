# Todo

Nothing is built. These are the decisions to make first, roughly in order.

Where rustmas already answered something, it says so. Those are not settled by
authority, they are settled by having been tried, sometimes twice. Reopen any of
them if C# argues otherwise, but know what you are arguing with.

## Next

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
