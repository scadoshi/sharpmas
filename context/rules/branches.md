## Branches

```
main        the tool, no solutions, no helpers   <- default, what people clone
<yours>     main plus your solutions             <- where the puzzle work happens
```

`main` is what a stranger gets from `git clone`. Your branch is `main` with the
solutions added on top. The owner's is called `scadoshi`; name yours whatever
you like and read that name wherever this file says it.

### The trap when switching branches

Untracked files survive a checkout, so files written on the solutions branch are
still sitting there after switching to `main`. A `git add -A` on `main` then
commits them, which has happened here more than once.

Check `git status` before staging on `main`, and prefer naming paths over
`add -A` when the working tree has been on the other branch.

### Keeping them in step

Additive, in one direction:

```sh
git checkout <yours>
git merge main
```

A change to the tool lands on `main` first, then merges down. Nothing is deleted
to produce a branch, so there is no drift to catch and no force push.

Check what a merge did before trusting it:

```sh
git diff main <yours> --name-status
```

It should list the solutions as added and nothing as removed. If the solutions
are missing from that list, the merge took them.

The cost is discipline: a tool fix noticed while writing a day belongs on `main`,
not here.

### What belongs where

Only on the solutions branch:

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
