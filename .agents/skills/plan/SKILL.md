# Plan

## Purpose

Create a small, reviewable implementation plan before meaningful code changes.

## Procedure

1. Read `AGENTS.md`.
2. Read `docs/requirements.md`.
3. Read relevant ADRs.
4. Inspect relevant implementation and tests.
5. Identify requirements affected by the change.
6. Identify architectural constraints.
7. Divide work into small vertical phases.
8. Define verification for each phase.
9. Save the plan under `.agents/plans/`.

## Plan Format

```markdown
# Plan: <title>

## Goal

What needs to change and why.

## Requirements

- REQ-...
- REQ-...

## Design

Short description of the intended approach.

## Files Likely Affected

- path
- path

## Phases

### Phase 1 — <name>

- [ ] implementation task
- [ ] tests
- [ ] verification

### Phase 2 — <name>

- [ ] implementation task
- [ ] tests
- [ ] verification

## Risks

- risk
- mitigation

## Done When

Concrete observable completion criteria.


## Rules

Keep plans proportional to the task.

A ten-line change does not need a ten-page plan.


---

# `.agents/skills/implement/SKILL.md`

```markdown
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