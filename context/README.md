# context

Facts about the repository, for a person or an AI getting to know it. It explains the repo and assumes nothing about who you are.

- [`architecture.md`](architecture.md) covers the layout, how a solve runs, and the cache on disk.
- [`services.md`](services.md) covers the two services the tool talks to and the etiquette it follows.
- [`design.md`](design.md) lists every design decision, a line or two each.

Getting started is in the repo README. [rustmas](https://github.com/scadoshi/rustmas) is the original this rebuilds. Its `context/design/` has the full reasoning, and its `references.md` has the service contracts as verified.

## Branches

`main` is the tool with no solutions, and it's what you clone to start.

- Solutions and their registry entries live on a personal branch layered on top of `main`.
- Changes flow one way: merge `main` down into the personal branch.
- The `scadoshi` branch is Scotty's, with his solutions and working notes, if you want worked examples.

## Do not

Read, cat, or print `.env`. It holds a personal session cookie.
