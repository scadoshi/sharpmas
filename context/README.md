# context

Facts about the repository, for a person or an AI getting familiar with it. It
explains the repo and assumes nothing about who you are.

- [`architecture.md`](architecture.md) is the layout, how a solve runs, and the
  cache on disk.
- [`services.md`](services.md) is the two services the tool talks to and the
  etiquette it follows.
- [`design.md`](design.md) is every design decision in a line or two each.

The repo README covers getting started. [rustmas](https://github.com/scadoshi/rustmas)
is the original this rebuilds; its `context/design/` and `references.md` hold
the full reasoning and the service contracts as verified.

## Branches

`main` is the tool with no solutions: what you clone to start. Solutions and
their registry entries live on a personal branch layered on top, with changes
flowing one way by merging `main` down. The `scadoshi` branch is Scotty's, with
his solutions and working notes, if you want worked examples.

## Do not

Read, cat, or print `.env`. It holds a personal session cookie.
