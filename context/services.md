# The two services

This is the short version. The full contracts, verified live and against the solver's source, are in `rustmas/context/references.md`.

## adventofcode.com

The site authenticates with the session cookie. It grades each part exactly once, and a second correct answer comes back as "already solved" instead of another confirmation.

It returns 200 for everything, wrong answers included. That has a few consequences:

- The client reads a submission's verdict from the response body.
- It checks for the direction hints before the generic wrong-answer phrase, since a "too high" reply contains both.
- An expired cookie redirects to the login page, which is also a 200. A page with zero puzzle articles is how the client spots it.

## The third-party solver

The solver has no auth, no accounts, and no memory, so it'll answer the same question as many times as you ask. That's why it works as a free check before submitting.

- Every failure is a 400 with the reason in the body.
- `Unsupported` in that body means it has no implementation for the puzzle.
- Three deployments run the same code. A 5xx means the host is down, not the solver, so the client moves on to the next one.

## Etiquette

The tool follows four rules:

1. Never re-download an input.
2. Never republish puzzle text.
3. Send a `User-Agent` that names a reachable contact.
4. Expect an escalating cooldown after wrong answers.

The cooldown is the reason submitting waits for a solver check.
