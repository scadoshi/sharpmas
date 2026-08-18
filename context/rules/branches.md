## Branches

```
main        the tool, no solutions, no helpers   <- default, what people clone
scadoshi    main plus solutions                  <- where the puzzle work happens
```

`main` is what a stranger gets from `git clone`. `scadoshi` is `main` with the
solutions added on top.

### Keeping them in step

Additive, in one direction:

```sh
git checkout scadoshi
git merge main
```

A change to the tool lands on `main` first, then merges down. Nothing is deleted
to produce a branch, so there is no drift to catch and no force push.

Check what a merge did before trusting it:

```sh
git diff main scadoshi --name-status
```

It should list the solutions as added and nothing as removed. If the solutions
are missing from that list, the merge took them.

The cost is discipline: a tool fix noticed while writing a day belongs on `main`,
not here.

### What belongs where

Only on `scadoshi`:

- `Domain/Solution/Year*/`, the days themselves
- `Domain/Solution/Common/`, the helpers days share
- the entries in `Solvers` in `Inbound/Solve/SolveRun.cs`

Not `YearTemplate/`, which is tooling for writing solutions rather than a
solution, and which a fresh clone of `main` should have. It is compiled on every
build so it cannot drift from `ISolution`.

`Common/` is deliberately not on `main`. It holds grid and direction helpers that
exist to serve puzzles, so a clone with no solutions has nothing to use them for,
and the template is meant to stand alone.

On `main`, and therefore on both: everything else, including `context/`.
