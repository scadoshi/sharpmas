## Commit Guidelines

- Concise, one-line messages (multi-line only when many changes)
- Group related files logically, one concern per commit
- No emojis
- Use `git diff` to understand changes before committing
- **Never** include AI-agent signatures in your commits.
    - Example: "Written with the help of Claude Opus 4.5"
    - Never commit with something like this in your message.

### Check before committing, not after

`dotnet test`, then `dotnet build` read for warnings. A warning here is worth
treating as a failure: several of the bugs recorded in the journal compiled
clean, and the ones that did warn said exactly what was wrong.

If a change touches instructions, follow them rather than reading them. In
rustmas the README's "adding a solution" steps went stale three times, twice
while someone was looking straight at them.

### Never leave a commit that does not build

Fold a fix into the commit that needs it rather than committing after it, so no
commit in history is broken.

### Committing across both branches

Not yet in force: sharpmas has only `main`. Once solutions get their own branch,
the rule is the one in `rustmas/context/rules/branches.md`. Tool changes land on
`main` first, then merge down, and the work happens in a worktree rather than by
switching branches:

```sh
git worktree add /tmp/wt main
# copy the changed files in, commit there
git worktree remove /tmp/wt
```

Only copy files that are identical on both branches. `git diff main <branch>
--name-only` lists the ones that are not, and those get the same edit by hand on
each side.
