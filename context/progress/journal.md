# Journal

Newest first.

## 2026-08-07 (later)

Started translating, working through rustmas file by file rather than designing
anything new. `Year`, `Day`, and `Part` exist under `Domain/Address/`. Nothing
else yet.

The mode is translation plus questions: read the Rust, write the C#, ask what
the idiom is when they diverge. Worth continuing that way, since the design
decisions are already made and recorded in `rustmas/context/design/`.

`Part` is left as a class wrapping an enum, which is not the idiom. Unwrapping
it to a bare enum plus an extension method is first on `../todo.md`, understood
but deliberately parked rather than rushed at the end of a session.

What came up so far, all C# facts rather than design choices:

- No free functions. Every method lives in a type, so a Rust module of loose
  functions becomes a `static class`, or the methods hang off the type they
  concern. `Year.DaysIn()` and `Year.Latest()` went the second way.
- Doc comments are `///` like Rust but XML rather than markdown, so
  `<summary>` and `<see cref="X"/>` rather than prose and `[`Type`]`. The
  `cref` names are compiler checked.
- Enums are named integers, not closed sets, so a `switch` over one needs a
  discard arm and the compiler cannot prove exhaustiveness. `(Part)99` is legal.
- Nullable comparison lifts: `int? == int` yields false rather than throwing, so
  `is_none_or` becomes `x is null || x == y`. Both `x < 5` and `x >= 5` are
  false when `x` is null, which is a trap.
- Ranges are not values. `Enumerable.Range(start, count)` takes a count rather
  than an end, and there is no `contains` on a range, so a bounds check is just
  two comparisons. That fed back into rustmas, where the range was doing the
  same thing more elaborately.
- Composition over inheritance. `Day` holds a `Year` rather than extending one.
  Inheriting would have collided on `Value` and made a `Day` substitutable
  wherever a `Year` was expected.
- `sealed` by default, which documents intent and lets the JIT devirtualize.

Two naming rules that differ in a way worth remembering: C#'s `To*` means
"converts to another representation" and says nothing about cost, while Rust's
`to_` specifically signals expense, with `as_` for free conversions. So the same
method is `ToWireValue()` in C# and should have been `wire_value()` in Rust,
which got renamed there.

## 2026-08-07

Scaffolded the solution: `Sharpmas` as a class library, `Sharpmas.Cli` as the
entry point referencing it, `Sharpmas.Tests` referencing the library. .NET 10
writes the new `.slnx` solution format rather than `.sln`. Template stubs
deleted, so the library and test project are empty.

Splitting rather than one console project because a test project referencing a
class library is the well-trodden path, while referencing an executable works
but is not. rustmas got its tests by keeping logic in the library and the binary
at four lines.

Still no real code. Refreshed this context against rustmas, which reached feature
complete and got restructured the same day, so several notes here described a
version that no longer exists.

Three things `todo.md` listed as open are now answered by having been tried:
one executable with subcommands rather than two, runtime input reading rather
than embedding, and a registry that can be queried without holding an input.

What changed in rustmas worth knowing before starting: the library is ports and
adapters now, the cache is a directory of plain files per day rather than one
document, inputs carry a hash of the cookie that fetched them, puzzle text is
cached per part, and the answer and verdict types were split by provenance after
timing exposed that one type was measuring one field while holding two others
from different sources.

## 2026-08-06

Repo created. README, this context directory, licence, and a .NET gitignore.
No code, no project structure, on purpose: the shape should be decided in C#
terms rather than transliterated from Rust.

rustmas reached feature complete the same day, so it is a working reference
rather than a half-built one. Its `context/` holds the design notes and, more
usefully, `references.md`, where both service contracts are recorded from live
probing and from reading the solver's source. That part is language independent
and should be read before any HTTP code gets written here.

[`../context/README.md`](../README.md) lists which rustmas decisions carry over
and which ones C# reopens. The short version: the two-client split, the
submission gate, filter semantics, input caching, and having no answer cache are
all language independent. Compile-time input embedding, the object-safety
reasoning, and the dispatch macro are Rust artefacts and should not be copied.
