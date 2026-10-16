## Commit Guidelines

- Write concise, one-line messages. Go multi-line only when there are many changes.
- Group related files logically, one concern per commit.
- Use no emojis.
- Read `git diff` to understand the changes before committing.
- Never include AI-agent signatures in your commits. A line like "Written with the help of Claude Opus 4.5" never goes in a message.

### Check before committing, not after

Run the checks in this order:

1. `dotnet test`.
2. `dotnet build`, and read it for warnings. Treat a warning as a failure: several bugs in this repo's history compiled clean, and the ones that did warn said exactly what was wrong.
3. `dotnet format`, since the build doesn't check `.editorconfig`.

CI in `.github/workflows/ci.yml` runs the same checks on every push to `main` or `scadoshi`, using `dotnet build -warnaserror` and `dotnet format --verify-no-changes`. A red run means one of these was skipped. Run them locally first rather than letting CI find it.

If a change touches instructions, follow them rather than reading them. In rustmas the README's "adding a solution" steps went stale three times, twice while someone was looking straight at them.

### Never leave a commit that does not build

Fold a fix into the commit that needs it instead of committing it afterward. That way no commit in history is broken.

### Committing across both branches

Tool changes land on `main` first, then merge down. [`branches.md`](branches.md) says what belongs where.

Do the `main` work in a worktree rather than by switching branches. That also avoids the trap of untracked files following a checkout.

1. Create the worktree:

   ```sh
   git worktree add /tmp/wt main
   ```

2. Copy the changed files in and commit there.
3. Remove it:

   ```sh
   git worktree remove /tmp/wt
   ```

Only copy files that are identical on both branches. `git diff main <branch> --name-only` lists the ones that aren't, and those get the same edit by hand on each side.
