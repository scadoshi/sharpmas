# context

Facts about the repository, plus who owns this branch and how he works. The
repo facts are one file per topic, shared with `main`; the personal sections
below exist only here.

- [`todo.md`](todo.md) is what is coming next. Read it first.
- [`progress/journal.md`](progress/journal.md) is dated session logs, newest
  first. The latest entries list the C# facts learned so far, worth skimming
  before answering a language question.

- [`architecture.md`](architecture.md) is the layout, how a solve runs, and the
  cache on disk.
- [`services.md`](services.md) is the two services the tool talks to and the
  etiquette it follows.
- [`design.md`](design.md) is every design decision in a line or two each.
- [`rules/`](rules/) is binding when working here: commit guidelines, doc
  comment style, and the branch model.

The repo README covers getting started. [rustmas](https://github.com/scadoshi/rustmas)
is the original this rebuilds; its `context/design/` and `references.md` hold
the full reasoning and the service contracts as verified.

## Branches

`main` is the tool with no solutions: what you clone to start. Solutions and
their registry entries live on a personal branch layered on top, with changes
flowing one way by merging `main` down. `rules/branches.md` has the model. The
`scadoshi` branch is Scotty's, with his solutions and working notes, if you
want worked examples.

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

Update `todo.md` and add a journal entry at the end of a working session.

## Do not

Read, cat, or print `.env`. It holds a personal session cookie.
