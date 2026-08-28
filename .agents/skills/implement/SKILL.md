# Implement

## Purpose

Execute one approved implementation plan without introducing unrelated changes.

## Procedure

1. Read the plan.
2. Confirm the current phase.
3. Read the affected requirements.
4. Inspect relevant code and tests.
5. Implement the smallest coherent behavior.
6. Add or update tests.
7. Run focused tests.
8. Run repository verification when the phase is complete.
9. Update plan checkboxes.
10. Stop if a meaningful architectural ambiguity appears.

## Rules

Do not:

- redesign unrelated areas
- silently change the plan
- hide requirements inside hard-coded branching
- skip tests because sample output works
- continue through an unresolved requirement ambiguity

## Completion

Report:

- files changed
- behavior implemented
- tests added or changed
- verification result
- unresolved concerns