# Plan: port the Filter type from rustmas

What: eager validation of the `-y`/`-d` flags, so an impossible filter errors
up front instead of sweeping the range and matching nothing in silence.

Target behaviour, copied from the rustmas terminal:

```
solve -y 2030          Error: year 2030 is outside 2015..=2025
solve -d 26            Error: day 26 is outside 1..=25
solve -y 2025 -d 13    Error: day 13 is outside 1..=12
```

The third line is the point: a paired filter reports the year's own day count.

## Order of work

1. **Errors first, they are the payoff.** Give `Year`'s constructor an
   exception message naming the given value and the live `Latest()`. Same for
   `Day`'s, naming the year's own `DaysIn()`. Today both throw
   `ArgumentOutOfRangeException` with default messages.
2. **`FinalDay` constant** (25) on `Day`. Two consumers: the loose day bound
   below, and `HasSecondPuzzle` (step 6).
3. **`Filter` type** in `Domain/Address/Filter.cs`. Holds `Year?` and `int?`.
   The day field is deliberately not a `Day`: a day filter with no year is not
   an address, since day 13 is valid in 2015 and not in 2025.

   `Filter.New(int? year, int? day)` throws on the first problem. Four cases:
   - nothing: fine
   - year alone: `new Year(y)` validates it
   - day alone: loose bound `1..=FinalDay`
   - both: `new Day(year, d)` proves the pair, keep the parts, discard the Day
4. **`Day.All()`** yielding every published day (the guts of today's `Each`),
   then **`Day.Matching(filter)`** filtering it. Delete `Each`; three call
   sites: `FetchRun`, `SolveRun.Run`, `SolveRun.PartCount`.
5. **Call sites** build the filter once at the top of each run. The throw
   surfaces through `Cli`'s existing catch, which prints the cause chain. No
   new plumbing.
6. **`HasSecondPuzzle` on `Day`** (`Value != FinalDay`), wired into
   `Inputs.EnsureEntry`'s chase-part-two gate. Without it a finished day 25
   costs a request every run, forever. Decision already made in rustmas: the
   day 25 closing note is never fetched; it is a congratulation, not a puzzle.

## Tests (mirror rustmas's, names theirs)

- accepts what the range allows: nothing, year alone, day 13 alone, 2015+25
- rejects either side out of range: 2030 alone, 26 alone
- judges the pair strictly: 13 alone ok, 2015+13 ok, 2025+13 rejected
- the error names the real bound: "1..=12" appears for 2025+13
- the pair filter yields exactly its one day, not just a count of one
- day 25 lacks a second puzzle, every other day has one

## Reference

rustmas: `src/lib/domain/address/filter.rs`, `day.rs`, `mod.rs` (the error
structs), `src/lib/inbound/input.rs` (the gate). Journal entries 2026-08-20.
