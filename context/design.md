# Design decisions

The concise version. The full reasoning, including rejected options, lives in
`rustmas/context/design/`, where these were settled before being rebuilt here.

- **Two clients, not one.** The services differ in auth, contract, and failure
  semantics. The split also shows that validating needs no cookie at all.
- **Submission gates on a solver check**, since a wrong answer costs a cooldown
  and the check is free. A puzzle the solver does not support submits anyway.
- **Year and day are filters.** Omitting one means all of them; one iterator
  serves every mode.
- **Inputs are read at runtime, never embedded**, so a fresh clone works with an
  empty cache.
- **The cache is plain files**, one directory per day, each file readable on its
  own. A structured document was tried in rustmas and read badly.
- **Puzzle text splits structurally.** One `<article>` per unlocked part, so
  counting them says which parts exist. No parsing of prose, no flag that can
  disagree with the text beside it.
- **No local answer cache.** "Already solved" from the site is the durable
  record of a star.
- **An absence means one thing.** Missing, empty, unset, and not-yet-fetched are
  named cases, never one null carrying four meanings. When a nullable would be
  ambiguous, split the cases into a type, as `AnswerResult` does.
- **Types split by provenance.** What a part computed, how long it took, and
  what each checker said come from different places, so they are different
  fields and different verdict types.
- **A day reports bad input by throwing from `Parse`.** That ends the day and
  leaves the others alone. Each part is caught separately, so one broken part
  does not hide the other's answer.
