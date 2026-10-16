# Comment Guidelines

A comment says what the code does, in the present tense, and only when the code doesn't say it itself.

The code explains itself. A comment covers the part it can't: the surprise, the invariant, the unit.

A function with a comment on every line has the wrong names. Fix the names.

## Do not write

- No history. Skip "previously", "used to", "now", "no longer", "was changed to", and leave out dates, commit hashes and issue numbers. The code has no past tense; git has the history.
- No situation. Skip "this is needed because we ran into" and "this prevents the bug where". If the code only makes sense with the story attached, the code is wrong, not the comment.
- No sermon. Skip "without this, X would happen, and then Y" and say what the line does. If the consequence is the part that isn't obvious, give it one short clause.
- No restatement. `/// <summary>Returns the name.</summary>` above `Name()` is noise, so delete it. When every member of a type explains itself, the type gets the summary and the members get none.
- No advice, unless it's an instruction the reader must follow, like "loopback only, do not widen".

## Do write

- What a caller can rely on: invariants, guarantees, ordering.
- The surprise: a reply that looks wrong but is right, an argument that is ignored, a close that looks early.
- Units, ranges and formats the type doesn't carry.

## Form

- Keep it to one line when it can be. A doc comment may run longer, with an example when the signature can't say it, and stops there.
- Open every file with a one-line `<summary>` on its top-level type.
- Give every public item a doc comment, unless it explains itself.
- Add a `<remarks>` block only when the summary can't say it.
- Use no em dashes, in code or in prose. Don't join two fragments with a spaced hyphen either. Use a sentence, a colon, or a comma.
- Use American spelling.

## Before and after

```csharp
// Bad: a past the reader does not need, and a sermon.
// Previously we re-downloaded the input every run, which hammered the site
// (see the 2025-12-03 journal entry). Without this check we would get banned.

// Good: what it does, and the one fact that is not obvious.
/// <summary>Returns a day's cached entry, downloading whatever is missing.</summary>
/// <remarks>Puzzle text is fetched again after part one is solved, since the site only serves part two then.</remarks>
```

```csharp
// Bad: restates the name.
/// <summary>Gets the elapsed time.</summary>
public TimeSpan Elapsed { get; }

// Good: nothing. The name says it.
public TimeSpan Elapsed { get; }
```

## Checking

Grep for the usual offenders:

```bash
grep -rnE 'previously|used to|no longer|without this|in order to|—' src tests --include='*.cs'
```

A hit isn't always wrong, since "used to" also means "used for". Read each one.

Then reread every comment you wrote and delete the ones a competent reader wouldn't miss.
