# Todo

## Where this is

Translating rustmas file by file. `Domain/Address/` has `Year`, `Day`, and
`Part`. `Domain/Solution/` has `Answer`, `AocVerdict`, `SolverVerdict`, and a
partial `Outcome`. Builds clean with no warnings. The approach is deliberate:
the design is settled and recorded in `rustmas/context/design/`, so this is
transliteration plus asking what the C# idiom is wherever the languages diverge.

Where rustmas already answered something, the notes below say so. Those are not
settled by authority, they are settled by having been tried, sometimes twice.
Reopen any of them if C# argues otherwise, but know what you are arguing with.

## Next

- **Finish `Outcome`.** It has the four properties and `WithVerdict`. Missing
  `WithSubmission`, which is the same shape, and `ToString`.

  `ToString` is the interesting one. The rule from rustmas: AOC's word
  supersedes the solver's, so a part AOC graded reads `starred` or `new star`
  rather than repeating that the solver agreed. Any other AOC reply shows next
  to what the solver thought. The line is the answer, then the notes in
  parentheses, then the elapsed time.

- **Decide the `Visual` newline.** rustmas prefixes art with a newline so it
  starts on its own line, and has a test asserting `"\n###"`. `Answer.ToString`
  returns it bare. Settle it while writing `Outcome.ToString`, since that is
  where the line layout gets decided.

- **Then `ISolution`.** The trait a day implements: parse once in the
  constructor, then `PartOne` and `PartTwo` read the parsed result. Rust's
  `Sized` and object-safety reasoning does not carry over, since C# interfaces
  dispatch dynamically.

- **Write the first tests.** `Sharpmas.Tests` still has none against rustmas's
  65. The two worth porting first cover invariants already hit by hand: that an
  unsubmittable answer never takes a verdict, and the `Outcome` display matrix
  including AOC superseding the solver.

  What earns a test is settled in rustmas and carries over: something more than
  one day depends on, or an error path nothing else exercises. Not the happy
  path of a day's puzzle logic, which is what `--validate` is for, and never a
  table of known answers as a regression guard.

- **Add `Answer.Unwritten`.** rustmas grew a fourth case after `None` turned out
  to mean three different things: no answer exists, nobody has written this part
  yet, and there is no such puzzle. `Unwritten` splits the second off and prints
  `(unwritten)`. `Answer.cs` currently has `Value`, `Visual`, and `None` only.

- **Close the hierarchies.** `Answer`, `AocVerdict`, and `SolverVerdict` are
  abstract records with sealed leaves, but nothing stops outside code adding a
  case. A `private Answer() { }` on the base fixes that, since only nested types
  can reach it. Worth doing once the shapes stop moving.

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
  without asking. Built in rustmas since, and confirmed working. The timing is
  the whole point: the cache is read at the top of the day loop and the
  submission happens further down, so the run that earns the star has already
  read a cache in which part two was still locked. Hanging the refetch off a
  `Correct` verdict on part one catches the one moment it is certain to have
  just unlocked. Reuses the "is part two missing" check, so a day that already
  has it costs a cache read and no request.
