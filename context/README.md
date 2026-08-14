# context (read me first)

Hand this dir to any AI assistant to work on `sharpmas` with full context.

## Where things are

- [`todo.md`](todo.md) is what is coming next. Read it first.
- [`progress/journal.md`](progress/journal.md) is dated session logs, newest
  first. The latest entry lists the C# facts learned so far, which is worth
  skimming before answering a language question.
- [`rules/commit_guidelines.md`](rules/commit_guidelines.md) is binding for any
  commit.
- [`rules/doc_comments.md`](rules/doc_comments.md) is binding for any doc
  comment. One line unless a reader would otherwise get it wrong.
- `design/` does not exist yet. Add it when a decision gets made, one file per
  topic, recording rejected options alongside chosen ones. Most decisions are
  already made in rustmas, so this stays empty until C# forces a different
  answer.

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

### Where it stands

```
sharpmas.slnx
src/Sharpmas/            class library, the whole tool
  Domain/Address/        Year.cs, Day.cs, Part.cs
  Domain/Solution/       Answer.cs, AocVerdict.cs, SolverVerdict.cs, Outcome.cs
src/Sharpmas.Cli/        console entry point, still Hello World
tests/Sharpmas.Tests/    xunit, references the library, no tests yet
```

The address types are translated, as are both verdicts and `Answer`. `Outcome`
is partial. Nothing reaches the network or the disk yet, the CLI does nothing,
and there are no tests. `todo.md` has the current state in detail.

### How it is being built

Translating rustmas file by file, in roughly its own dependency order. Scotty
writes the C# and asks what the idiom is wherever the languages diverge, then
the answers get recorded in the journal.

That means the job here is usually **explaining C#, not designing**. The design
is settled and lives in `rustmas/context/design/`. When something looks wrong in
the translation, say so, but check the Rust first: it is probably deliberate and
the reason is probably written down.

Answer as an educator. He knows programming and knows Rust deeply. He does not
know C#. Translate between the two, name the idiom, say when there is no clean
analogue.

## The reference implementation

[rustmas](https://github.com/scadoshi/rustmas) is the finished version, cloned
at `~/Developer/rustmas`. Two branches: `main` is the tool with no solutions,
`scadoshi` adds his. The design notes and `references.md` are on both.

It is feature complete. Fetch, solve, validate against a third-party solver, and
submit for stars all work, with 65 tests and every service behaviour driven live
rather than assumed. Day one of every year is solved except 2019, which is being
saved to do in one run.

Both repos follow the same branch rule, which is written up in
`rustmas/context/rules/branches.md`: `main` is the tool, a personal branch adds
solutions on top, and changes flow one way by merging `main` down. sharpmas has
only `main` so far.

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

## What carries over

Settled in rustmas and language independent. Several of these were reversed once
before they stuck, and the reasons are in `rustmas/context/design/`.

**Two clients, not one.** AOC is authenticated and grades each part once. The
solver is anonymous and answers the same question forever. They differ in auth,
contract, and failure semantics, so one type covering both hid more than it
saved. Splitting them also made the cookie's reach obvious: validating needs no
authentication at all.

**Submission gates on a solver check**, because a wrong answer to AOC costs an
escalating cooldown and the solver check is free. An unsupported puzzle submits
anyway, since that is the live-event case where being ahead of the solver is
exactly when submitting matters.

**One command, one subcommand per mode.** Two separate executables were tried
and collapsed. They were split for organisation, nothing is deployed
separately, and the split only bought a longer invocation.

**Year and day are filters rather than lookups.** Omitting one means all of
them, so the four flag combinations need no matching, and one shared iterator
serves every mode.

**Inputs are read at runtime, never embedded.** Solving downloads what it does
not have, so a fresh clone works with an empty cache and the tool has no
build-order requirement.

**One directory of plain files per day**, rather than one structured document.
An input and a page of puzzle text both read badly escaped onto a single JSON
line, and every file being openable on its own is worth more than a struct.

**Inputs carry a hash of the cookie that fetched them.** They are account
specific, so swapping accounts silently invalidates them, and nothing catches
it: the same file answered `280` one day and `138` the next. A mismatch
refetches the input and keeps the puzzle text, which is identical for everyone.

**Puzzle text splits structurally, not textually.** The day page holds one
`<article class="day-desc">` per unlocked part, so counting them says which
parts you have. No parsing of "the answer to part one" and no flag that can
disagree with the text beside it.

**No local answer cache.** It looked mandatory and was not. AOC is stateful, so
"already solved" is itself the durable record of a star, and the fact that
looked irreplaceable was one request away.

**One line of output per part**, carrying the answer, what each checker said,
and how long it took.

**An absence must mean one thing.** The single most productive bug hunt in
rustmas was finding every place where something missing could mean two
different things with nothing to decide which. A day cached without part two's
text meant either "still locked" or "never fetched". `Answer.None` meant no
answer, or nobody wrote it, or no such puzzle. A cache file read as blank meant
empty or absent. An unreadable cookie was indistinguishable from an unset one.
Each was one line to fix and each had produced a wrong answer or a silent
skip. C# makes this easier to get wrong, since `null` is the same token for all
of them, so name the cases rather than reaching for a nullable.

**Types split by provenance.** What a part computed, how long it took, and what
each checker said come from three different places, so they are three fields
rather than one type pretending to be coherent. Same for verdicts: the solver
and AOC can each say things the other cannot, and one shared type meant every
match carried impossible arms.

## What C# reopens

- `include_str!` and compile-time embedding were rejected in rustmas anyway, so
  runtime reading is the shared answer rather than a Rust workaround.
- The `Sized` trait and object safety reasoning is Rust only. C# interfaces are
  reference types and dispatch dynamically, so the question disappears.
- Dispatch is a hand-written registry in rustmas because Rust has no runtime
  reflection. C# has it, so reflection over an interface or a source generator
  are both available. Whatever it is, adding a day should be one small edit, and
  the registry should be answerable without holding an input: rustmas counts
  what a run would submit and reports an unwritten day by asking it.
- Newtype validation carried over cleanly, as it turned out. `Year` and `Day`
  are `sealed class`es with get-only properties and validating constructors, and
  `Day` takes a built `Year` rather than a number, so the cascade holds. The one
  C# hazard: a get-only auto-property left unassigned silently stays at
  `default` rather than failing to compile, which bit once already.
- `Result` and `Option` become exceptions and nullable references, which changes
  how errors are threaded rather than just how they are spelled. Note where
  rustmas leans on this: a missing cache file is `None` rather than an error, so
  "not downloaded yet" is an ordinary answer.
- Rust's newtypes cannot be deserialised without bypassing their constructors,
  which is why rustmas stores plain files and parses on read. C# has the same
  trap with any serialiser that constructs field by field.

## Do not

Read, cat, or print `.env`. It holds a personal session cookie.
