## Branches

```
main        the tool, no solutions, no helpers   <- default, what people clone
<yours>     main plus your solutions             <- where the puzzle work happens
```

`main` is what a stranger gets from `git clone`. Your branch is `main` with the solutions added on top.

The owner's branch is called `scadoshi`. Name yours whatever you like, and read that name wherever this file says it.

### The trap when switching branches

Untracked files survive a checkout. Files written on the solutions branch are still sitting there after you switch to `main`, and a `git add -A` on `main` then commits them. That has happened here more than once.

- Check `git status` before staging on `main`.
- When the working tree has been on the other branch, name paths instead of using `add -A`.

### Keeping them in step

Changes are additive and flow one way. Merge `main` into your branch:

```sh
git checkout <yours>
git merge main
```

A change to the tool lands on `main` first, then merges down. Nothing is deleted to produce a branch, so there's no drift to catch and no force push.

Check what a merge did before trusting it:

```sh
git diff main <yours> --name-status
```

It should list the solutions as added and nothing as removed. If the solutions are missing from that list, the merge took them.

The cost is discipline. A tool fix you notice while writing a day belongs on `main`, not here.

### What belongs where

Only on the solutions branch:

- `Domain/Solution/Year*/`, the days themselves.
- The contents of `Domain/Solution/Common/`, but not the folder itself. `main` ships the folder and a note saying what earns a place in it, so a fresh clone has somewhere to put shared helpers.
- The entries in `Solvers` in `Inbound/Solve/SolveRun.cs`.

`YearTemplate/` stays on `main`. It's tooling for writing solutions rather than a solution, and a fresh clone of `main` should have it. It compiles on every build, so it can't drift from `ISolution`.

`Common/` ships empty on `main`. The folder and its note are scaffolding; the types inside are solution code.

- Write a `Common/` type for the second day that wants it, not the first.
- Give it tests, since a break there corrupts every day at once.

Everything else lives on `main`, and therefore on both branches. That includes `context/`.
