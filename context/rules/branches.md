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
- the contents of `Domain/Solution/Common/`, but not the folder itself. `main`
  ships the folder and a note saying what earns a place in it, so a fresh clone
  has somewhere to put shared helpers
- the entries in `Solvers` in `Inbound/Solve/SolveRun.cs`

Not `YearTemplate/`, which is tooling for writing solutions rather than a
solution, and which a fresh clone of `main` should have. It is compiled on every
build so it cannot drift from `ISolution`.

`Common/` ships empty on `main`: the folder and its note are scaffolding, the
types inside are solution code. Write one the second day that wants it rather
than the first, and give it tests, since a break there corrupts every day at
once.

On `main`, and therefore on both: everything else, including `context/`.
