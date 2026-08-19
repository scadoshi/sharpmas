# context

Background for working on sharpmas. This is the owner's branch, so alongside
the repo facts it carries who he is and how he works; `main`'s version of this
file has the repo facts alone.

## Where things are

- [`todo.md`](todo.md) is what is coming next. Read it first.
- [`progress/journal.md`](progress/journal.md) is dated session logs, newest
  first. The latest entry lists the C# facts learned so far, which is worth
  skimming before answering a language question.
- [`rules/`](rules/) is binding for commits and doc comments.

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
loud, coach and nudge instead of jumping to code. Answer as an educator:
translate between the two languages, name the idiom, say when there is no clean
analogue.

Verify claims rather than asserting them. Several design decisions across both
repos changed because something got probed or read rather than assumed, and at
least two confident assertions turned out to be wrong.

## What sharpmas is

Advent of Code tooling in C#: fetch puzzle inputs, run solutions, check answers
against an independent third-party solver, and submit them for stars. The tool
is complete and works end to end. Solutions are yours to write.

It is a rebuild of [rustmas](https://github.com/scadoshi/rustmas), whose
`context/design/` and `references.md` record the design reasoning and both
service contracts in full. This file carries the concise version.

## Layout

```
sharpmas.slnx
src/Sharpmas/            class library, the whole tool
  Domain/                puzzles; knows nothing of HTTP, files, or the CLI
    Address/             which puzzle: Year, Day, Part
    Solution/            what a run produced: Answer, AnswerResult, Outcome,
                         Solved, the two verdicts, and ISolution
      Common/            helpers more than one day needs, empty to start
      YearTemplate/      copy this to start a year
  Inbound/               the CLI, and what each subcommand does
    Fetch/               downloading into the cache
    Solve/               running, validating, submitting, the day registry
  Outbound/              the world outside
    Client/              AocClient, SolverClient, and the environment they read
    Store/               the cache on disk
  Extensions/            small helpers on BCL types
src/Sharpmas.Cli/        entry point, one line
tests/Sharpmas.Tests/    xunit, mirrors the source tree
```

Ports and adapters: `Domain/` imports nothing outside itself, `Inbound/` is the
way in, `Outbound/` is the way out. The repo README covers running it and adding
a solution; `rules/` is binding for commits and doc comments.

## The two services

- **adventofcode.com** is authenticated by session cookie and grades each part
  exactly once. It returns 200 for everything, so a submission's verdict is read
  from the response body, direction hints before the generic wrong-answer
  phrase. A second correct answer reads as "already solved".
- **The third-party solver** needs no auth and answers as often as asked, which
  is what makes it usable as a check before submitting. Every failure is a 400
  with the reason in the body.

Etiquette the tool follows: never re-download an input, never republish puzzle
text, send a User-Agent that names a reachable contact, and expect an escalating
cooldown after wrong answers.

## Design decisions, concisely

- **Two clients, not one.** The services differ in auth, contract, and failure
  semantics. The split also shows that validating needs no cookie at all.
- **Submission gates on a solver check**, since a wrong answer costs a cooldown
  and the check is free. A puzzle the solver does not support submits anyway.
- **Year and day are filters.** Omitting one means all of them; one iterator
  serves every mode.
- **Inputs are read at runtime, never embedded**, so a fresh clone works with an
  empty cache.
- **The cache is plain files**, one directory per day, each file readable on its
  own. Inputs carry a hash of the cookie that fetched them, because inputs are
  account specific and a swapped account otherwise goes unnoticed.
- **Puzzle text splits structurally.** One `<article>` per unlocked part, so
  counting them says which parts exist. Part two absent means still locked.
- **No local answer cache.** "Already solved" from the site is the durable
  record of a star.
- **An absence means one thing.** Missing, empty, unset, and not-yet-fetched are
  named cases, never one null carrying four meanings. When a nullable would be
  ambiguous, split the cases into a type, as `AnswerResult` does.
- **Types split by provenance.** What a part computed, how long it took, and
  what each checker said come from different places, so they are different
  fields and different verdict types.

## Branches

`main` is the tool, with no solutions: what you clone to start. Solutions and a
day registry entry per solved day live on a personal branch layered on top, with
changes flowing one way by merging `main` down. `rules/branches.md` has the
model. The `scadoshi` branch is Scotty's, with his solutions and working notes,
if you want worked examples.

## Do not

Read, cat, or print `.env`. It holds a personal session cookie.
