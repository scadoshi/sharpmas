# Design decisions

The short version. These were settled in rustmas before being rebuilt here, and `rustmas/context/design/` has the full reasoning, rejected options included.

### Two clients

The two services differ in auth, contract, and failure semantics, so each gets its own client. The split also makes it obvious that validating needs no cookie.

### Submitting waits for the solver

A wrong answer costs a cooldown and a solver check costs nothing, so a submission only goes out after the check. The exception is a puzzle the solver doesn't support, which gets submitted anyway.

### Year and day are filters

Leaving one out means all of them. One iterator serves every mode.

### Inputs load at runtime

Nothing is embedded at build time, so a fresh clone works with an empty cache.

### The cache is plain files

There's one directory per day, and each file is readable by itself. rustmas tried a structured document first, and it read badly.

### Puzzle text splits on structure

The page has one `<article>` per unlocked part, so counting them tells you which parts exist.

- Nothing parses the prose.
- No flag sits beside the text where it could disagree with it.

### No local answer cache

The site's "already solved" reply is the durable record of a star.

### Each absence has one meaning

Missing, empty, unset, and not yet fetched are separate named cases. One null never carries all four meanings. When a nullable would be ambiguous, split the cases into a type, the way `AnswerResult` does.

### Types split by where the data comes from

What a part computed, how long it took, and what each checker said all come from different places. So they're different fields, and the two checkers get different verdict types.

### Bad input throws from `Parse`

A throw from `Parse` ends that day and leaves the other days alone. Each part is caught on its own, so one broken part can't hide the other part's answer.
