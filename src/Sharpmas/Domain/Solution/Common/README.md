# Common

Helpers more than one day needs.

It's empty on `main` on purpose. Add a type here when the second day wants it, not the first, because a helper written for one puzzle ends up shaped by that puzzle.

Grid and geometry work tends to land here, since Advent of Code comes back to it every year:

- a cartesian point
- a grid cell with rows and columns
- a compass direction
- a quarter turn

Test everything in this folder. A break here corrupts every day at once. A single day's logic doesn't need tests, because `--validate` covers it.

Use the namespace `Sharpmas.Domain.Solution.Common` to match the folder.
