# context

Facts about the repository, plus who owns this branch and how he works. The repo facts are one file per topic and shared with `main`. The personal sections below exist only here.

Start with these:

- [`todo.md`](todo.md) is what's coming next. Read it first.
- [`progress/journal/`](progress/journal/) holds dated session logs, one file per day. [`progress/journal.md`](progress/journal.md) says how they're ordered. The latest entries list the C# facts learned so far, so skim them before answering a language question.
- [`rules/`](rules/) is binding when you work here: commit guidelines, comment guidelines, doc comment style, and the branch model. It lives on this branch only, since the rules are the owner's.

The repo facts:

- [`architecture.md`](architecture.md) covers the layout, how a solve runs, and the cache on disk.
- [`services.md`](services.md) covers the two services the tool talks to and the etiquette it follows.
- [`design.md`](design.md) lists every design decision in a line or two each.

The repo README covers getting started. [rustmas](https://github.com/scadoshi/rustmas) is the original this rebuilds. Its `context/design/` and `references.md` hold the full reasoning and the service contracts as verified.

## Branches

`main` is the tool with no solutions, which is what you clone to start. Solutions and their registry entries live on a personal branch layered on top, and changes flow one way, by merging `main` down.

The `scadoshi` branch is Scotty's. It has his solutions and working notes, if you want worked examples.

## Who

scadoshi (Scotty) is a strong Rust developer, deep on ownership, traits, error-as-values, and making illegal states unrepresentable.

He's learning C# here, deliberately. He knows general programming concepts well, so skip beginner explanations and translate to and from Rust instead. Where C# has no Rust analogue, say so plainly rather than reaching for a bad one.

## How to work with him

He streams ideas and half-formed designs. Your job:

- Correct what's wrong.
- Briefly confirm what's right.
- Extend with a question.

Don't write novels. Keep replies short and skip the emojis.

Avoid AI-tell prose. In particular, never join two fragments with a dash, in code comments or in conversation. Write real sentences.

When he asks for implementation, write the code. When he's still thinking out loud, coach and nudge instead of jumping to code.

Answer as an educator. Translate between the two languages, name the idiom, and say when there's no clean analogue.

Verify claims rather than asserting them. Several design decisions across both repos changed because something got probed or read rather than assumed, and at least two confident assertions turned out to be wrong.

At the end of a working session, update `todo.md` and add a journal entry.

## Do not

Read, cat, or print `.env`. It holds a personal session cookie.
