# AI Journey

AI was used as an engineering collaborator throughout this challenge.

The goal was not to maximize generated code.
The goal was to use AI where it improved reasoning speed while keeping
requirements, business rules, and verification under deterministic control.

## How AI Was Used

An AI coding assistant was used for requirement analysis, implementation
exploration, test design, and review of rule interactions. The selected
material interaction is recorded in `prompts.md`; routine autocomplete and
boilerplate are omitted.

## How AI Was Controlled

AI output is treated as a proposal, not evidence.

Important behavior is verified through:

- deterministic business logic
- unit tests
- integration tests
- expected-output comparison
- requirement traceability
- independent review

The repository also defines explicit agent responsibilities under `.agents/`.

## Files

`prompts.md`

Contains selected interactions where AI materially influenced the solution.

`learnings.md`

Contains mistakes, corrections, trade-offs, and lessons from using AI during
the challenge.

Routine autocomplete and boilerplate interactions are intentionally omitted.
