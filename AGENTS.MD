# Credit Engine — AI Engineering Guidelines

## Mission

Build the BTG Pactual Credit Engine with correctness, simplicity,
explainability, and maintainability.

AI agents are contributors to the engineering process.

They are not the source of truth.

---

## Authority Order

When sources conflict, use this order:

1. Original challenge requirements
2. `docs/requirements.md`
3. Human-approved requirement decisions
4. Accepted Architecture Decision Records
5. Approved implementation plans

Tests and verification evidence prove implementation behavior.

They do not override requirements or approved decisions.

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

Completion requires concrete evidence such as:

- unit tests
- integration tests
- expected-output comparison when the official artifact is available
- deterministic verification
- implementation evidence
- documented architectural decisions when relevant

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

### 6. Challenge fidelity beats framework convenience

Do not change externally visible challenge semantics merely because
a framework, language convention, library, or abstraction would make
implementation easier.

Preserve:

- required API behavior
- required field semantics
- priority rules
- financial behavior
- stateless execution
- expected validation behavior

When the challenge is ambiguous, surface the ambiguity instead of silently
normalizing it into a preferred design.

---

## Development Workflow

For meaningful implementation changes:

Architect

→ Implementer

→ Deterministic Verification

→ Reviewer

→ Implementer when fixes are required

→ Deterministic Verification

Challenge Auditor is a submission-compliance role.

Run it when:

- preparing a release or submission candidate
- explicitly requested by an approved plan
- a significant requirement-compliance concern exists

The same AI product may execute multiple roles when necessary,
but role boundaries must remain explicit.

When tooling allows it:

- Reviewer should use a fresh context from the Implementer.
- Final Challenge Auditor should use a fresh context from both
  implementation and previous audits.

Reviewer and Challenge Auditor must not rely on the Implementer's
self-assessment as evidence.

---

## Before Implementation

1. Read `docs/requirements.md`.
2. Read relevant ADRs.
3. Read the approved implementation plan.
4. Inspect the existing implementation.
5. Identify affected requirements and expected tests.

If no approved plan exists, or if the plan is no longer valid,
stop and route the work back to the Architect.

The Implementer must not silently rewrite the plan to fit the implementation.

Do not start a large implementation from an ambiguous requirement.

---

## During Implementation

1. Work in small vertical slices.
2. Keep domain logic isolated.
3. Add or update tests together with behavior.
4. Avoid unrelated refactoring.
5. Preserve challenge semantics exactly.
6. Record meaningful AI-assisted decisions when they occur.
7. Do not expand scope simply because the implementation makes it easy.
8. Prefer the smallest implementation that satisfies the approved design.

If implementation reveals a requirement or architecture conflict,
stop and escalate rather than resolving it silently.

---

## Before Declaring a Milestone Complete

Run the repository verification command.

Then:

1. Confirm deterministic verification passes.
2. Compare the implemented scope against the approved plan.
3. Confirm affected requirements have concrete evidence.
4. Run independent review when required by the plan or workflow.
5. Record meaningful AI Journey evidence when applicable.

"Looks correct" is not verification.

---

## Before Submission

1. Run deterministic verification from a clean repository state.
2. Run an independent technical review.
3. Resolve or explicitly disposition material findings.
4. Review requirement-to-evidence traceability.
5. Curate the AI Journey.
6. Run a fresh Challenge Audit.
7. Submit only when the audit concludes `READY_FOR_SUBMISSION`.

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

Workflow sequencing may be defined under:

`.agents/workflows/`

---

## Agent Responsibility Boundaries

Architect owns interpretation and planning.

Implementer owns execution.

Reviewer owns independent criticism.

Challenge Auditor owns submission compliance.

No agent owns all four.

Agents must not cross these responsibility boundaries silently.

If completing a task requires assuming another agent's authority,
stop and escalate instead.

---

## Escalation Rule

An agent MUST stop rather than continue when completing its task
would require:

- inventing business behavior
- changing an approved requirement
- changing an approved architecture decision
- overriding the original challenge contract
- fabricating unavailable challenge artifacts
- assuming authority assigned to another agent role

When escalation is required:

1. Describe the blocking issue.
2. Identify the conflicting source or missing decision.
3. Explain why continuing would require an unauthorized assumption.
4. Route the issue to the appropriate role or human decision.

Do not work around the conflict silently.

---

## Independent Review

Independent review is intended to reduce self-validation bias.

Reviewer must prioritize findings in this order:

1. correctness
2. challenge requirements
3. edge cases and boundaries
4. architecture
5. tests
6. maintainability
7. unnecessary complexity

Reviewer should not silently modify the implementation being reviewed.

Findings should be supported by concrete evidence.

When possible, use a fresh context for review.

---

## Challenge Audit

Challenge Auditor evaluates submission compliance, not implementation ownership.

The audit should map:

requirement

→ concrete evidence

→ verification or test evidence when applicable

Audit statuses are:

- `VERIFIED`
- `PARTIAL`
- `MISSING`
- `AMBIGUOUS`

Final conclusions are:

- `READY_FOR_SUBMISSION`
- `NOT_READY_FOR_SUBMISSION`

Challenge Auditor must not implement missing functionality while auditing.

---

## Technology Constraints

The production implementation must use:

- .NET 10
- C#
- ASP.NET Core for the REST API

The technology stack is a human decision and must not be changed
without explicit approval.

Libraries, testing frameworks, project structure, and implementation
details should be selected proportionally to the challenge requirements.


---

## AI Journey

The evaluator explicitly wants meaningful AI collaboration,
not a raw transcript.

Record interactions when AI materially influenced:

- architecture
- rule representation
- requirement interpretation
- edge-case discovery
- testing strategy
- important trade-offs
- a failed approach
- an AI mistake that required correction
- independent review that changed the implementation
- complexity that was deliberately removed

Prefer entries that demonstrate:

AI proposal

→ critical evaluation

→ problem or missing assumption

→ iteration

→ final engineering decision

→ evidence

Do not log routine boilerplate conversations.

Do not fabricate prompts, mistakes, decisions, or outcomes after the fact.

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