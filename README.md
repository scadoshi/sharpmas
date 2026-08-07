# sharpmas

Advent of Code tooling in C#, rebuilt from
[rustmas](https://github.com/scadoshi/rustmas).

Nothing here yet except the plan.

## What it will do

`fetch` downloads puzzle inputs into `inputs/<year>/<NN>.txt`, filtered by year
and day, skipping whatever is already on disk.

`solve` runs the solutions with the same filters, and can check each answer
against an independent third-party solver on the way.

`submit` posts answers to adventofcode.com. It only sends what the solver agreed
with, because a wrong answer there earns a cooldown that gets longer every time
you trip it.

## Why

The C# is the point. rustmas already works, so the puzzle logic is a solved
problem and the real question is what happens to a design when the language
changes underneath it. Some of it should survive intact. Some of it only existed
because Rust has no runtime reflection, or because `include_str!` was sitting
right there.

## Start here

[`context/`](context/) covers what this is, how to work on it, and which rustmas
decisions were settled for good reasons versus which ones C# gets to reopen.

## License

MIT. See [LICENSE](LICENSE).
