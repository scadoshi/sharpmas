# Journal

Newest first.

## 2026-08-07

Still no code. Refreshed this context against rustmas, which reached feature
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
