# Todo

Nothing is built. These are the decisions to make first, roughly in order.

## Next

- Pick the project shape. One console app, or a class library plus two
  executables the way rustmas splits `fetch` and `solve`? The split earned its
  keep there because the two have different triggers and only one needs a
  cookie.
- Decide how inputs reach a solution: read at runtime, or embedded as a
  resource. Runtime reading is probably right, and it removes rustmas's ordering
  problem where the project cannot build until inputs are downloaded.
- Decide how a day registers itself. Rust needed a macro because it has no
  runtime reflection. C# has it, so reflection over an interface, or a source
  generator, are both available. Whatever it is, adding a day should be one
  small edit.
- Argument parsing. `System.CommandLine`, or something smaller. Needs `--year`
  and `--day` as filters, plus `--validate`, `--submit`, and `--yes`.

## Soon

- The two HTTP clients. Contracts are already recorded in
  `rustmas/context/references.md`, verified rather than guessed, so this is
  mostly transcription.
- How answers and verdicts are modelled. Rust used an enum with the verdict
  folded into the submittable variant, so a visual answer carrying one was
  unrepresentable. C# has no direct equivalent, so this needs a real decision
  rather than a transliteration.
- Pull solutions across from `csharp-aoc`, which stays as it is.

## Later

- Timing per part, split from parse time. Never got built in rustmas either.
