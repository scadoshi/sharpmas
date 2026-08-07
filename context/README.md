# context (read me first)

Hand this dir to any AI assistant to work on `sharpmas` with full context.

## Where things are

- [`todo.md`](todo.md) is what is coming next.
- [`progress/journal.md`](progress/journal.md) is dated session logs, newest
  first.
- [`rules/commit_guidelines.md`](rules/commit_guidelines.md) is binding for any
  commit.
- `design/` does not exist yet. Add it when a decision gets made, one file per
  topic, recording rejected options alongside chosen ones.

Update `todo.md` and add a journal entry at the end of a working session.

## Who

scadoshi (Scotty) is a strong Rust developer, deep on ownership, traits,
error-as-values, and making illegal states unrepresentable.

C# is the language being learned here, deliberately. He knows general
programming concepts well, so skip beginner explanations of programming and
translate to and from Rust instead. Where C# has no Rust analogue, say so
plainly rather than reaching for a bad one.

## How to work with him

He streams ideas and half-formed designs. Your job is to correct what's wrong,
briefly confirm what's right, and extend with a question. Don't write novels.
Keep replies short and skip the emojis.

Avoid AI-tell prose. In particular, never join two fragments with a dash, in
either code comments or conversation. Write real sentences.

When he asks for implementation, write the code. When he's still thinking out
loud, coach and nudge instead of jumping to code.

Verify claims rather than asserting them. In rustmas, several design decisions
changed because something got probed or read rather than assumed, and at least
two confident assertions turned out to be wrong.

## What sharpmas is

Advent of Code tooling in C#: fetch inputs, run solutions, check answers against
a third-party solver, submit for stars.

Nothing is built yet. No project structure has been chosen, and that is
deliberate: the shape should be decided in C# terms rather than transliterated.

## The reference implementation

[rustmas](https://github.com/scadoshi/rustmas) is the working version, cloned at
`~/Work/rustmas`. Its `context/` directory holds the design notes, the journal,
and `references.md`, which is the highest-value file here.

**Read `rustmas/context/references.md` before writing any HTTP code.** It records
both service contracts, verified live and against source rather than guessed:

- adventofcode.com returns **200 for everything**, wrong answers included, so
  the verdict is entirely in the response body. Match on the direction hint
  before the generic wrong-answer phrase, since a directional reply contains
  both. The direction hint is optional.
- The third-party solver returns **400 for every failure**, with the reason in
  the body. `Unsupported` means it has no implementation for that puzzle.
- AOC grades each part exactly once. After that it reports "already solved"
  rather than confirming again.
- Etiquette: do not re-download inputs, do not republish puzzle text, send a
  `User-Agent` that makes you reachable, and expect an escalating cooldown after
  wrong answers.

That file saves a day of probing and several wrong guesses. None of it is
language specific.

## What carries over, and what C# reopens

Settled in rustmas, and language independent:

- Two clients, not one. AOC is authenticated and grades once; the solver is
  anonymous and repeatable. They differ in auth, contract, and failure
  semantics.
- Submission gates on a solver check, because a wrong answer costs a cooldown.
  An unsupported puzzle submits anyway, since that is the live-event case.
- Year and day flags are filters rather than lookups, so omitting one means all.
- Inputs are cached by existing on disk. Never re-download, never overwrite.
- No local answer cache. It looked mandatory and was not: AOC is stateful and
  reports "already solved" itself.
- One line of output per part, carrying the answer and what each checker said.

Rust specific, and needs deciding fresh:

- `include_str!` embeds inputs at compile time, which is why rustmas cannot
  build before inputs are downloaded. C# has embedded resources, but reading at
  runtime is the more natural default and removes that ordering problem.
- The `Sized` trait and object safety reasoning is Rust only. C# interfaces are
  reference types and dispatch dynamically, so the whole question disappears.
- Dispatch in rustmas is a `macro_rules!` generating one match arm per day,
  because Rust has no runtime reflection. C# does, so registration by reflection
  or a source generator is available and probably better.
- Newtype validation (`Year` wrapping into `Day`, private fields, constructor
  only) maps onto C# less directly. Records with private constructors and static
  factories get close.
- `Result` and `Option` become exceptions and nullable references, which changes
  how errors are threaded rather than just how they are spelled.

## Do not

Read, cat, or print `.env`. It holds a personal session cookie.
