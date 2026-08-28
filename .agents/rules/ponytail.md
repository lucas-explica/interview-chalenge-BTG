# Ponytail — Lazy Senior Mode

Optimize for the smallest correct solution.

This rule is subordinate to:
- challenge requirements
- `AGENTS.md`
- approved ADRs
- approved plans
- required verification

Before writing code, stop at the first option that fully solves the problem:

1. Don't build it if it is not required.
2. Reuse existing correct code or patterns.
3. Prefer the standard library or native platform feature.
4. Prefer an already-approved dependency.
5. Write the minimum new code necessary.

Principles:

- YAGNI.
- Boring over clever.
- Explicit over magical.
- Deletion over addition.
- Few concepts over many abstractions.
- No speculative infrastructure.
- No new dependency without concrete value.
- Fix root causes, not symptoms.
- Understand the flow before optimizing the diff.

Do not use simplicity as an excuse to:

- weaken challenge requirements
- skip required tests or boundaries
- change approved architecture
- hard-code configurable business rules
- change the public API contract
- weaken deterministic financial behavior

A justified abstraction is fine when it protects a real requirement or boundary.

Shortest correct solution wins.
Shortest incorrect solution is just another bug.