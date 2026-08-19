## Doc Comments

One line. Go longer only when a reader would otherwise get it wrong.

```csharp
/// <summary>The submittable text, or null if there is none.</summary>
public string? Value { get; }
```

That is the shape to aim for: what it is or what it gives you, in a sentence,
no ceremony. Most members in this repo need nothing more.

Ported from rustmas, where the same rule is in force. Everything here is about
writing, not about C#, so the two files agree except in syntax.

### When a second sentence earns its place

Add one only for something the signature cannot say and a reader would
otherwise get wrong:

- A decision that looks like a mistake until explained. `ToString` is
  `sealed override` on the abstract record, which stops every derived record
  generating its own and shadowing it. Without a line saying that is
  deliberate, the next person unseals it.
- A rejected alternative that will otherwise be proposed again.
- A trap. Throwing, an argument that is ignored, an ordering that matters.
- A cross-reference that saves a search.

If the extra sentence only restates the signature in prose, cut it.

### What not to write

- No `<param>`, `<returns>`, or `<exception>` tags. This is a puzzle runner,
  not a published package. Say it in the summary.
- No restating the name. `/// <summary>Creates a new Day.</summary>` is noise.
- No documenting the obvious property. A get-only `Elapsed` needs nothing.
- No repeating a parent's doc on every case of a hierarchy. Say it once on the
  base record.
- No history. Describe the code as it is, never how it got there. "Used to be a
  class", "this used to throw" belong in a commit message or working notes. A
  reader arriving cold needs to know what the code does now.

`<summary>` on one line is fine and preferred. The three-line expanded form is
tooling default, not a requirement.

### Where the long-form goes instead

- your own working notes for what happened and when.
- `rustmas/context/design/` for why the design has its shape. Most decisions
  were settled there, so link rather than restate.
- `rustmas/context/references.md` for the two service contracts.

A doc comment growing into an essay is usually a design note trying to escape.

### Tests

A test name should carry the intent, so most tests need no doc at all. Write one
only when the test encodes a decision the assertions do not show.
