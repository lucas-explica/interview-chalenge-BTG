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

