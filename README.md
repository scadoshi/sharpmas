# sharpmas

Advent of Code tooling in C#. The same tool as
[rustmas](https://github.com/scadoshi/rustmas), rebuilt on .NET.

Nothing is written yet. This repo is the starting point, not a port in progress.

## What it will do

Three things, the same three rustmas does:

- **fetch**: download puzzle inputs to `inputs/<year>/<NN>.txt`, filtered by
  year and day, skipping anything already on disk.
- **solve**: run solutions, filtered the same way, optionally checking answers
  against an independent third-party solver.
- **submit**: post answers to adventofcode.com for stars, gated on that check so
  a known-wrong answer never earns a cooldown.

## Why it exists

Partly to have the tool in C#, partly as real practice in a language being
learned deliberately. The problem is already solved once, so the interesting
part is not the puzzle logic, it's how the same design lands in a different
language: what carries over, what needs a different shape, and what was a Rust
idiom rather than a good idea.

## Start here

[`context/`](context/) has everything an AI assistant or a returning human needs:
what this is, how to work on it, which decisions are already settled in rustmas,
and which ones C# reopens.

## License

MIT. See [LICENSE](LICENSE).
