# AI Journey

AI was used as an engineering collaborator throughout this challenge.

The goal was not to maximize generated code.
The goal was to use AI where it improved reasoning speed while keeping
requirements, business rules, and verification under deterministic control.

## Tools

This section will be updated with the tools actually used during development.

Examples may include:

- OpenAI Codex — implementation, exploration, review
- ChatGPT — architecture discussion and requirement analysis
- other coding assistants when useful for independent review

## How AI Was Used

AI may assist with:

- requirement analysis
- architecture alternatives
- implementation planning
- code generation
- edge-case discovery
- test generation
- independent review
- challenge compliance auditing

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