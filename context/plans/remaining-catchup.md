# Plan: the rest of the rustmas catch-up

Smaller items after the Filter port, each independent.

- **`Answer.Unwritten`.** Fourth case on `Answer`: `None` currently covers both
  "no answer exists" and "nobody wrote this part". `Unwritten` prints
  `(unwritten)`, and the `YearTemplate` stubs switch to returning it so a stub
  cannot read as a finished part with nothing to say. One record, one
  `ToString` arm, one display test, template update.
- **Close the hierarchies.** `private` constructor on the bases of `Answer`,
  `AnswerResult`, `AocVerdict`, `SolverVerdict`; only nested types can reach
  it, which is as close as C# gets to a sealed sum. Shapes have stopped moving,
  so the wait-for-stability condition is met.
- **Check `SolveRun`/`FetchRun` shape** against rustmas's run-into-module fold.
  Likely nothing to do; confirm rather than assume.

Done and landed already, for orientation: provenance renames, `LazyAocClient`
move, `ParsedIn`/`PartOne`/`PartTwo`/`TotalElapsed`.
