# The two services

Full contracts, verified live and against the solver's source, are in
`rustmas/context/references.md`. This is the short version.

## adventofcode.com

Authenticated by session cookie. Grades each part exactly once; a second
correct answer reports "already solved" rather than confirming again.

Returns **200 for everything**, wrong answers included, so a submission's
verdict is read from the response body. Direction hints are checked before the
generic wrong-answer phrase, since a "too high" reply contains both. An expired
cookie redirects to the login page, which also comes back 200: zero puzzle
articles on the page is the tell.

## The third-party solver

No auth, no accounts, no memory, so it answers the same question as often as
asked. That is what makes it usable as a free check before submitting. Every
failure is a **400 with the reason in the body**; `Unsupported` means it has no
implementation for that puzzle. Three deployments run the same code, so a 5xx
is the host rather than the solver, and the client tries the next one.

## Etiquette

Never re-download an input, never republish puzzle text, send a `User-Agent`
naming a reachable contact, and expect an escalating cooldown after wrong
answers. The tool follows all four; the cooldown is why submission gates on a
solver check.
