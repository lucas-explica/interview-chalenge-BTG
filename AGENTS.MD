# Credit Engine — AI Engineering Guidelines

## Mission

Build the BTG Pactual Credit Engine with correctness, simplicity,
explainability, and maintainability.

AI agents are contributors to the engineering process.
They are not the source of truth.

The source of truth is:

1. Challenge requirements
2. `docs/requirements.md`
3. Accepted Architecture Decision Records
4. Approved implementation plans
5. Executable tests and verification evidence

If implementation or tests conflict with the challenge requirements,
do not change the requirement to make the implementation pass.

---

## Engineering Principles

### 1. Business rules are data-driven

Business rules must be represented as configuration or structured data
whenever the challenge expects them to change independently from application logic.

Do not encode the rule tables as deeply nested `if/else` statements.

The evaluation algorithm may be code.
The business values and conditions it evaluates should be data.

---

### 2. Domain logic is independent from transport

The classification and credit calculation domain must not depend on:

- HTTP
- controllers
- serialization frameworks
- infrastructure concerns

The REST API is an adapter around the domain.

---

### 3. Financial behavior is deterministic

Do not delegate financial calculations, rounding, classification,
or rule evaluation to an LLM.

All calculations must be deterministic and testable.

---

### 4. Requirements require evidence

A requirement is not considered implemented because an agent says it is.

Completion requires evidence such as:

- unit tests
- integration tests
- expected-output comparison
- static verification
- documented architectural decision

See `docs/requirements.md`.

---

### 5. Prefer explicitness over clever abstractions

This is a focused credit engine, not a general-purpose rules platform.

Do not introduce:

- distributed orchestration
- generic workflow engines
- databases unless required
- message brokers
- unnecessary runtime AI dependencies
- abstractions without a concrete requirement

Design for extension where it is justified, but implement only what
the problem requires.

---

## Development Workflow

For meaningful changes:

Architect
→ Implementer
→ Reviewer
→ Challenge Auditor

The same agent may execute multiple roles when the tool does not support
independent subagents, but responsibilities must remain separated.

When possible, review should use a fresh context.

---

## Before Implementation

1. Read `docs/requirements.md`.
2. Read relevant ADRs.
3. Inspect the existing implementation.
4. Create or update a plan under `.agents/plans/`.
5. Identify affected requirements and expected tests.

Do not start a large implementation from an ambiguous requirement.

---

## During Implementation

1. Work in small vertical slices.
2. Keep domain logic isolated.
3. Add or update tests together with behavior.
4. Avoid unrelated refactoring.
5. Preserve challenge semantics exactly.
6. Record meaningful AI-assisted decisions when they occur.

---

## Before Declaring Completion

Run the repository verification command.

Then:

1. Confirm tests pass.
2. Compare implementation against `docs/requirements.md`.
3. Run an independent review.
4. Run the challenge audit.
5. Update relevant AI Journey documentation.

"Looks correct" is not verification.

---

## Agent Responsibilities

Canonical agent contracts live under:

`.agents/agents/`

Available roles:

- Architect
- Implementer
- Reviewer
- Challenge Auditor

Reusable procedures live under:

`.agents/skills/`

---

## AI Journey

The evaluator explicitly wants meaningful AI collaboration,
not a raw transcript.

Record interactions when AI materially influenced:

- architecture
- rule representation
- edge-case discovery
- testing strategy
- important trade-offs
- a failed approach
- an AI mistake that required correction

Do not log routine boilerplate conversations.

Files:

- `ai-journey/README.md`
- `ai-journey/prompts.md`
- `ai-journey/learnings.md`

---

## Tool Portability

This repository must not depend on a specific AI coding product.

`AGENTS.md` and `.agents/` are the canonical agent-facing source.

IDE-specific files may reference or expose these assets,
but they must not become a second source of truth.

The project must remain fully buildable and testable without an AI tool.