# Journal

Newest first.

## 2026-08-17 (later)

`Solved` is written, and `SolverClient.ValidateAnswer` is close. `Solve<T>` is a
stub in `Outbound/Client/Solve.cs` holding a signature and nothing else, because
validating needed a client to validate against first.

`Solved` is a plain class with three `required` init properties and `Total` as a
computed property. A computed property recomputes on every read rather than
caching, which is right here since it adds three `TimeSpan`s.

### Where `required` actually belongs

Got this backwards once and the compiler settled it. `required` obliges the
*caller* to supply a value, so it cannot coexist usefully with a default:
`new SolverClient()` with a `required` property fails `CS9035`, which means the
initialiser can never run. Dropping `required` is what makes a default real.
It earns its place when there is no sensible default and the compiler should
nag, which fits `Solved` and not `SolverClient`.

A related one, also confirmed rather than assumed: `required` forces the setter
to be at least as visible as the type, so `required` with a `private init` is
`CS9032`. There is no way to have `required` and keep the factory as the only
door.

Construction order matters with `init`. A caller using an object initialiser
assigns *after* the constructor body has run, so a constructor that reads
`UserAgent` to set a header would see the default rather than the override, and
the header and the property would disagree. Get-only properties assigned in the
constructor close that, which is what `SolverClient` does now.

### Two bugs that compiled clean

Both found by running the code rather than reading it, which is worth repeating.

- **`Uri + string` is string concatenation, not URI composition.**
  `Uri.ToString()` normalises an authority-only URI to carry a trailing slash,
  so `baseUrl + "/solve/..."` produced `https://advent.fly.dev//solve/...`. The
  relative constructor `new Uri(baseUrl, "solve/...")` composes properly.
- **Interpolating a type with no `ToString` prints the type name.**
  `$"{day.Year}"` gave `Sharpmas.Domain.Address.Year` inside the request URL.
  `Year` and `Day` both override `ToString` now, which fixes it everywhere at
  once rather than at one call site. rustmas relies on `Display` for the same
  reason.

### C# facts

- **`const` is compile-time only**: primitives, `string`, enums, `null`. An
  array cannot be one. `static readonly` is the answer, with the caveat that it
  freezes the reference and not the contents. `ImmutableArray<T>` or
  `FrozenSet<T>` if that matters.
- **A member shadows a type of the same name.** A property called `UserAgent`
  made `UserAgent.FromEnv()` resolve to the property, failing with `CS0236`
  since a field initialiser cannot reach an instance member. Renaming the static
  class to `Env` fixed it, and `Env` is where the session cookie will live too.
- **URL is a subset of URI**, identifying by location. The absolute versus
  relative distinction is separate and handled by `UriKind`. Naming follows the
  type, so `Uri`, matching the BCL's `baseUri` and `requestUri`.
- **Relational patterns for status ranges**: `(int)response.StatusCode is >= 400
  and < 500`. This is where `and` earns its keep.
- `Console.Error.WriteLine` is `eprintln!`.
- **`using` is scoped `Drop`, made explicit.** It guarantees `Dispose` at block
  exit including on an exception, and the narrowed scope is a consequence rather
  than the point. `Dispose` releases what the GC does not manage or will not
  release promptly; it is not about memory. Rust drops automatically because
  ownership fixes the timing, and C# needs the keyword because the GC decides
  instead.
- **`ParseAdd` validates against the RFC grammar** for `User-Agent`. The
  conventional AoC contact string often will not parse, and
  `TryAddWithoutValidation` is the way around it.

### `ValidateAnswer`

Structure follows rustmas: read the body before classifying, since every solver
failure is a 400 with the reason in the body and any 5xx is the hosting platform
rather than the solver. Numeric answers compare as numbers and report a
direction, anything else compares for equality only, because a text answer
cannot be high or low. Parsing is `long`, not `int`: a 32-bit overflow does not
error, it silently downgrades to the text comparison and loses the direction.

The response is deliberately not wrapped in `using`. Reading the body to
completion is what returns the connection to the pool, so what is left is a
small managed object. Worth adding back if anything ever stops reading the full
body or starts making many calls.

### Next

Finish `ValidateAnswer`, then `Solve<T>`.

1. **Transport failures still escape the loop.** `HttpRequestException` from the
   post, and `TaskCanceledException` from a timeout, both propagate instead of
   trying the next host, which is most of the reason three hosts are listed. The
   shape: grab `StatusCode` and the body inside a `try`, classify outside, and
   `continue` from the catch. Note that `IsSuccessStatusCode` goes out of scope
   with the response, so success becomes `(int)status is >= 200 and < 300`.
2. There is a typo in the final throw: "answser".
3. Then `Solve<T>`: `T.Parse`, time the parse and each part separately, catch
   per part into `AnswerResult`, and let a parse failure fly. The stub in
   `Solve.cs` takes an `HttpClient` and should take a `SolverClient`.

## 2026-08-17

`ISolution` is written, and 2015 day 1 implements it so the shape can be seen
against a real day rather than argued about.

```csharp
public interface ISolution<TSelf> where TSelf : ISolution<TSelf>
{
    static abstract TSelf Parse(string input);
    Answer PartOne();
    Answer PartTwo();
}
```

The self-referencing type parameter is C# hand-rolling what Rust has built in.
Every Rust trait is implicitly parameterised by the implementing type, so
`S::new(input)` inside a generic just works. C# interfaces have no `Self`, so
the type has to be passed to its own interface. The pattern is called CRTP if it
needs looking up.

`static abstract` is the pair that makes it work, and both halves are
load-bearing. `abstract` says the implementer supplies the body, which is
redundant on instance members but not here, since interfaces are allowed static
members that already have one. `static` says no instance is needed, which is the
point: a day cannot produce itself from an instance of itself.

The consequence is that `Parse` is unreachable through an interface reference,
because a static member has nothing to dispatch on. It can only be called as
`T.Parse(input)` from inside a generic constrained on `ISolution<T>`. That is
exactly where `Solve<T>` will call it, which is the same place rustmas calls
`S::new`.

### How failure travels

Three places, three mechanisms, settled after mistaking them for one:

- A day reports a bad input by **throwing**. A constructor cannot return
  failure, and this is what C# exceptions are for.
- `Solve<T>` catches each part separately into **`AnswerResult`**, so one broken
  part does not hide the other's answer.
- A failure from `Parse` is **not caught** by `Solve<T>` at all. It ends the day
  and the runner reports it, which is what rustmas does: `S::new(input)?`
  propagates and `run.rs` prints that the day failed.

Nearly built a `SolutionResult` to mirror `AnswerResult` before noticing they
answer different questions. `AnswerResult` exists because two parts must fail
independently and a settled outcome has to be *stored* for printing. Parsing has
one caller which immediately gives up, so a result type there would be built to
be unwrapped on the next line.

Worth keeping straight: throwing is how C# propagates, a closed union is how it
stores. Rust uses one type for both and that is what made the distinction easy
to miss.

### C# facts

- **`required` forces the setter to be at least as visible as the type.**
  Confirmed by compiling it: `required` with a `private init` is CS9032. So
  `required` necessarily means callers can bypass the factory and build the type
  by object initialiser. Fine for day 1, where the raw input is the whole state.
  Any day that parses into fields wants a private constructor instead, so
  `Parse` is the only door.
- **`Aggregate` is `fold`.** With a seed it matches; without one it uses the
  first element and throws on an empty sequence. Nothing in LINQ short-circuits,
  so there is no `try_fold` and an early return stays a loop.
- **`string` is already `IEnumerable<char>`.** `ToCharArray()` allocates a copy
  for nothing.
- `Index()` yields `(index, item)` pairs, which is `enumerate()`.

### Layout

`Domain/Solution/Year2015/Day01/Puzzle.cs`, with the namespace mirroring the
path, so every day has its own `Puzzle` exactly as rustmas has one per module.
The registry will have to name them qualified, since it cannot import several
namespaces that all define `Puzzle`. Nothing else imports a day, so the cost
stops there. C# has no `pub(super)`, so day-local helpers get `internal`, which
is the whole assembly rather than the day.

Caught one bug worth naming, since rustmas made the same one on 2016 day 1.
Part two returned the final floor when the basement was never reached, which is
part one's answer wearing part two's hat. It returns `Answer.None` now.

### Next

`Solve<T>`: call `T.Parse`, time the parse and each part separately, catch per
part into `AnswerResult`, and let a parse failure fly. Then the registry.

## 2026-08-15

First tests. 18 of them, over `Answer` and `Outcome`, which is the display
matrix and the invariant that an unsubmittable answer never takes a verdict.

`dotnet test` had been reporting "No test is available in ...", which read like
a wiring problem and was not. The test project had the runner, the SDK, and the
project reference all along. That message is what xunit says when the assembly
contains no `[Fact]`, so the fix was writing one.

Two things about the tests themselves:

- **`TimeSpan.Zero` beats rustmas's `notes()` helper.** That helper exists only
  because the Rust tests use a real duration and then have to strip the timing
  off the end before comparing. A zero duration renders as `[0ns]`, so the whole
  line can be asserted at once and the helper is unnecessary.
- **A single exception would not have tested `Causes` at all.** The chain test
  nests one inside another, since the loop is only wrong in a way that shows
  with two.

Settled the `Visual` newline while the tests were being written, which is where
todo.md predicted it would surface. Art gets a newline on both sides, not just
in front, so the timing lands on its own line rather than reading as another row
of the picture. rustmas leads only and is being changed to match.

That left a trailing space before `[0ns]` on the art case. The first fix asked
whether the answer was `Visual`, which works and says the wrong thing: the rule
is "the line already ended", and `Outcome` otherwise never inspects which
`Answer` case it holds. `if (!message.EndsWith('\n'))` is the same behaviour
without the coupling, and stays correct if anything else ever ends in a newline.

## 2026-08-14

`Outcome` is finished: `GetValue`, `WithVerdict`, `WithSubmission`, and
`ToString`. Two extension classes came out of it, and the reason both exist is
that Rust gave away for free what C# makes you write.

`AnswerResult` stands in for `Result<Answer>`. An abstract record with `Ok` and
`Err` cases, so a part that failed is held rather than propagated and one broken
part does not hide the other's answer. The alternative was `Answer?` next to
`Exception?`, which allows both null and both set, and neither means anything.

Three C# facts learned writing it:

- **Extension methods are a static class with `this` on the first parameter.**
  The first attempt subclassed `Exception`, which compiles and does nothing:
  the method then only exists on a type nothing throws. Inheritance is not
  involved, which is exactly why it works on types you do not own.
- **A `using` for the namespace is what brings extensions into scope**, not one
  for the type. Both call sites failed with "does not contain a definition for"
  until `using Sharpmas.Extensions;` went in, and the error names the type
  rather than the missing import.
- **`TimeSpan.Nanoseconds` is the component, not the total.** It reports
  0 to 999 within the current microsecond. The other three read as `Total*`, so
  the odd one out is easy to miss. `TimeSpan` resolution is 100ns anyway, so
  that branch can only land on multiples of 100 and will never read quite like
  Rust's.

`or` patterns are worth knowing: `Answer.Visual or Answer.None => null` collapses
two arms. `and` and `not` exist too, which Rust has no equivalent for. The trade
is that C# cannot prove a hierarchy is closed, which is why every switch here
carries an `UnreachableException` arm that Rust would not need. Richer
combinators over a set the compiler cannot verify.

Found one thing worth changing in rustmas rather than porting. The notes in
`Display for Outcome` were built as a `Vec<String>` and joined, but every arm of
the match has a fixed count of zero, one, or two, so the join was joining a list
whose shape was decided one line above it. Building the string directly in each
arm is simpler in both languages. Fixed on rustmas `main` and merged down.

### Next

Tests. `dotnet test` currently finds no tests in the assembly, so that needs
sorting before anything can be ported. `todo.md` has the order.

## 2026-08-10 (later)

`Domain/Solution/` now holds `Answer`, `AocVerdict`, `SolverVerdict`, and a
partial `Outcome`. Directory is singular, matching the rename in rustmas.

Everything below was verified by compiling and running it against the built
library, not by reading. Four of these were live bugs that the build did not
catch.

- **A `ToString` override on an abstract record never runs.** Every derived
  record generates its own, which sits below yours in the chain, so virtual
  dispatch picks theirs. `new AocVerdict.Cooldown("1m 0s")` printed
  `Cooldown { Wait = 1m 0s }` rather than the switch's text. `sealed override`
  fixes it: derived records skip generating one when the base forbids it.

  Keep the switch on the base rather than one override per case. A case added
  later without a line then hits the discard arm and throws, instead of quietly
  printing `Timeout { }`.

- **Never pass `this` as the value to `ArgumentOutOfRangeException` from inside
  `ToString`.** `Message` formats that value by calling `ToString()` on it,
  which re-enters the switch and throws again. The result is not a bad message,
  it is no message: `Cannot print exception string because Exception.ToString()
  failed`. Use `UnreachableException` with `GetType().Name`, which cannot
  recurse.

  `ArgumentOutOfRangeException` is right when a real argument was out of range,
  as in `Part.WireValue`. It is wrong for an impossible branch with no argument
  in sight.

- **`required` is about initialization, not nullability.** Nullability says
  which values are legal; `required` says the caller must supply one. They are
  mutually exclusive with a constructor: `required` plus a constructor gives
  `CS9035`, because assigning in a constructor does not count as setting it.
  Use `required` for plain value bags built with object initializers, a
  constructor when construction has to validate or derive something. Not both.

- **`Math.Sign` is the translation of Rust's `Ordering`.** C# has no three-case
  comparison type; the convention is the sign of an `int`, and `CompareTo` only
  promises a sign, never `-1`/`0`/`1`. Matching on those literals throws on real
  input, since `string.CompareTo` returns things like `-2`. `Math.Sign` narrows
  the open type to a closed one, which is the general shape for translating Rust
  enums into C#: narrow at the boundary, then switch.

- **An enum cannot be given a `ToString`.** It already inherits one from
  `System.Enum`, and extension members only fill gaps rather than shadow
  existing members. An extension named `ToString` compiles and is silently never
  called, since instance members always win overload resolution. Enums also
  cannot declare members at all, so there is nothing to override.

  `SolverVerdict` became a record hierarchy for this reason, matching
  `AocVerdict`. Value equality survives the change, since records generate `==`.

- **`private set` and `private init`.** An accessor takes its own access
  modifier. `{ get; private set; }` is mutable inside, read-only outside;
  `{ get; }` says even the class does not reassign it. Use `private set` only
  when the class really does write after construction.

- **`With` means non-destructive in C#**, as in records' `with` expression and
  the BCL's `WithComparer`. `WithVerdict` now returns a copy rather than
  mutating, which also matches rustmas, where `with_verdict(mut self)` consumes
  and returns a value.

  The property must be `private init`, not `init`. A public `init` lets a caller
  write `outcome with { Verdict = ... }` and skip the submittable-answer check;
  `private init` blocks that with `CS0272` while `this with { ... }` still works
  inside the class.

  C# cannot reproduce Rust's consumption. The un-verdicted original stays alive
  and usable. `private init` at least stops anyone building a wrong one.

### Where this stops

`Outcome` has `Answer`, `Elapsed`, `Verdict`, `Submission`, and `WithVerdict`.
Still missing `WithSubmission` and the `ToString` carrying the rule that AOC's
word supersedes the solver's, so a starred part reads `starred` rather than
repeating that the solver agreed.

## 2026-08-10

`Part` unwrapped from a class wrapping a nested enum into a bare enum plus
`PartExtensions`, which is where this session's C# discoveries came from.

- **Extension members (C# 14).** An `extension(Part part)` block inside a static
  class declares the receiver once and holds properties as well as methods,
  where the old `this Part part` form was methods only. Drop the parameter name,
  `extension(Part)`, and the members are static, so an enum can carry
  `Part.All`. `ToWireValue()` became the `WireValue` property. Verified
  compiling and running on `net10.0`. It lowers to the same static method, so it
  is spelling rather than a new mechanism.
- Properties signal cheap and side-effect free. Anything touching the network or
  disk stays a method, which matters once the clients land.
- Extension class naming is `PartExtensions`. The BCL spells it out and
  pluralises it, since the class is named for the set it holds.
- **Records.** `record` alone means `record class`; `record struct` is the
  opt-in. Positional parameters work at any nesting level, so nesting the cases
  inside the base buys namespacing, not payloads.
- **Closing a hierarchy** is a `private` constructor on the abstract base plus
  nested cases, since only a nested type can reach it. That is the closest C#
  gets to a Rust enum.
- Even so, **exhaustiveness is not checked**. A closed hierarchy with every case
  handled still warns `CS8509`, because `null` always inhabits a reference type.
  A throwing discard arm is permanent ceremony here.
- **Validated records must redeclare the property as `get;`, never `init;`.**
  Tested both: with `init`, a `with` expression assigns the property directly
  and silently bypasses the constructor's validation. With `get;` the same
  expression fails to compile. Records are otherwise the better newtype, since
  value equality is what you want from one.
- Guards: `ArgumentOutOfRangeException.ThrowIfLessThan` and friends generate
  messages naming the parameter and value, via `CallerArgumentExpression`.
  `Year` and `Day` both use them now, and both throw
  `ArgumentOutOfRangeException` rather than `InvalidOperationException`, which
  means "wrong state for this call" and was the wrong type for a bad argument.
- Shape to repeat in every validated type: guards first, then assign. Assigning
  before validating leaves a half-built object alive for a few lines.

`Duration` translates to `TimeSpan`.

### Where this stops

Mid-file in `Domain/Solutions/`, deliberately. `Answer.cs`, `Outcome.cs`, and
`SolverVerdict.cs` exist and are half built. `Outcome` in particular is a stub
with public fields and no constructor. This does not compile cleanly and that is
fine.

The reason for stopping: modelling `Outcome` surfaced a layering problem **in
rustmas**, not here. `domain/solutions/outcome.rs` imports `AocVerdict` and
`SolverVerdict` from `outbound::client`, and `solution.rs` imports
`SolverClient` from the same place. A domain type depending on an adapter type
is backwards for ports and adapters, and it has been true for a while without
anyone minding.

Next session is a reorganisation of rustmas to fix that, then the translation
picks up from a version worth copying. Do not translate the current layering.

## 2026-08-07 (later)

Started translating, working through rustmas file by file rather than designing
anything new. `Year`, `Day`, and `Part` exist under `Domain/Address/`. Nothing
else yet.

The mode is translation plus questions: read the Rust, write the C#, ask what
the idiom is when they diverge. Worth continuing that way, since the design
decisions are already made and recorded in `rustmas/context/design/`.

`Part` is left as a class wrapping an enum, which is not the idiom. Unwrapping
it to a bare enum plus an extension method is first on `../todo.md`, understood
but deliberately parked rather than rushed at the end of a session.

What came up so far, all C# facts rather than design choices:

- No free functions. Every method lives in a type, so a Rust module of loose
  functions becomes a `static class`, or the methods hang off the type they
  concern. `Year.DaysIn()` and `Year.Latest()` went the second way.
- Doc comments are `///` like Rust but XML rather than markdown, so
  `<summary>` and `<see cref="X"/>` rather than prose and `[`Type`]`. The
  `cref` names are compiler checked.
- Enums are named integers, not closed sets, so a `switch` over one needs a
  discard arm and the compiler cannot prove exhaustiveness. `(Part)99` is legal.
- Nullable comparison lifts: `int? == int` yields false rather than throwing, so
  `is_none_or` becomes `x is null || x == y`. Both `x < 5` and `x >= 5` are
  false when `x` is null, which is a trap.
- Ranges are not values. `Enumerable.Range(start, count)` takes a count rather
  than an end, and there is no `contains` on a range, so a bounds check is just
  two comparisons. That fed back into rustmas, where the range was doing the
  same thing more elaborately.
- Composition over inheritance. `Day` holds a `Year` rather than extending one.
  Inheriting would have collided on `Value` and made a `Day` substitutable
  wherever a `Year` was expected.
- `sealed` by default, which documents intent and lets the JIT devirtualize.

Two naming rules that differ in a way worth remembering: C#'s `To*` means
"converts to another representation" and says nothing about cost, while Rust's
`to_` specifically signals expense, with `as_` for free conversions. So the same
method is `ToWireValue()` in C# and should have been `wire_value()` in Rust,
which got renamed there.

## 2026-08-07

Scaffolded the solution: `Sharpmas` as a class library, `Sharpmas.Cli` as the
entry point referencing it, `Sharpmas.Tests` referencing the library. .NET 10
writes the new `.slnx` solution format rather than `.sln`. Template stubs
deleted, so the library and test project are empty.

Splitting rather than one console project because a test project referencing a
class library is the well-trodden path, while referencing an executable works
but is not. rustmas got its tests by keeping logic in the library and the binary
at four lines.

Still no real code. Refreshed this context against rustmas, which reached feature
complete and got restructured the same day, so several notes here described a
version that no longer exists.

Three things `todo.md` listed as open are now answered by having been tried:
one executable with subcommands rather than two, runtime input reading rather
than embedding, and a registry that can be queried without holding an input.

What changed in rustmas worth knowing before starting: the library is ports and
adapters now, the cache is a directory of plain files per day rather than one
document, inputs carry a hash of the cookie that fetched them, puzzle text is
cached per part, and the answer and verdict types were split by provenance after
timing exposed that one type was measuring one field while holding two others
from different sources.

## 2026-08-06

Repo created. README, this context directory, licence, and a .NET gitignore.
No code, no project structure, on purpose: the shape should be decided in C#
terms rather than transliterated from Rust.

rustmas reached feature complete the same day, so it is a working reference
rather than a half-built one. Its `context/` holds the design notes and, more
usefully, `references.md`, where both service contracts are recorded from live
probing and from reading the solver's source. That part is language independent
and should be read before any HTTP code gets written here.

[`../context/README.md`](../README.md) lists which rustmas decisions carry over
and which ones C# reopens. The short version: the two-client split, the
submission gate, filter semantics, input caching, and having no answer cache are
all language independent. Compile-time input embedding, the object-safety
reasoning, and the dispatch macro are Rust artefacts and should not be copied.
