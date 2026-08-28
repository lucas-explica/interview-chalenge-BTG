# BTG Credit Engine Delivery Workflow

## Purpose

This is the canonical delivery sequence for the BTG Pactual Credit Engine.

It orchestrates the existing agent and skill contracts; it is guidance stored
in the repository, not a runtime, state machine, or agent framework.

Read `AGENTS.md` first. The authority order defined there applies throughout
the workflow.

Use the role contracts in `.agents/agents/` and the reusable procedures in
`.agents/skills/`; this document defines sequencing only.

---

## Lifecycle

### Phase 1 — Requirements discovery, ambiguity resolution, architecture, and approved plan

- **Agent:** Architect (`.agents/agents/architect.md`)
- **Skill:** plan (`.agents/skills/plan/SKILL.md`)
- **Context:** fresh
- **Objective:** Establish an unambiguous, minimal design and an executable implementation plan before production work begins.

- **Scope:** Read the original challenge material, `docs/requirements.md`, relevant ADRs, the existing implementation and tests, and the repository harness. Identify requirement boundaries, priority rules, data-driven configuration, domain/API separation, financial invariants, validation behavior, and evidence needed for each requirement. Surface ambiguity rather than resolving it silently.

- **Expected artifacts:** An approved plan under `.agents/plans/`; ADRs only for meaningful alternatives or trade-offs; updated requirement traceability where needed. No production implementation.

#### Human Decision Gate

Before architecture decisions or implementation planning:

1. Report all material ambiguities found in the challenge.
2. Report any interpretation that would change observable system behavior.
3. Report missing official challenge artifacts or incomplete contracts.
4. Separate:
   - explicit challenge requirements
   - derived engineering implications
   - ambiguities
   - implementation decisions left to us
5. Identify which items require human judgment.

If any unresolved ambiguity requires human judgment:

- STOP
- do not create or approve architecture decisions based on an assumption
- do not create the final implementation plan
- report the blocking question and reasonable alternatives
- wait for an explicit human decision

After the human decision is provided:

- record the approved decision in the appropriate repository artifact
- resume Phase 1
- complete architecture and planning based on the approved interpretation

Do not silently resolve ambiguous challenge behavior.

- **Deterministic quality gate:** Every material requirement is traceable to a planned behavior and verification approach; architecture boundaries and unresolved decisions are explicit; no fabricated sample or official challenge artifact is treated as available; all human-decision blockers are resolved before Phase 1 completes.

- **Failure routing:** Requirement or contract ambiguity routes to the Architect and, where a human decision is required, the human owner. A missing official artifact routes to the Architect/human for clarification; do not create substitute expected outputs.

- **AI Journey Capture rule:** Record only meaningful requirement interpretations, architectural proposals, corrections, or trade-offs in `ai-journey/prompts.md`; add a reusable lesson to `ai-journey/learnings.md` only when one exists. Otherwise report `NO_JOURNEY_ENTRY`.

- **Next phase:** Phase 2 only after the implementation plan is approved and no material ambiguity blocks implementation.

---

### Phase 2 — Small vertical-slice implementation

- **Agent:** Implementer (`.agents/agents/implementer.md`)
- **Skill:** implement (`.agents/skills/implement/SKILL.md`)
- **Context:** continue from the approved plan
- **Objective:** Implement the smallest coherent behavior that satisfies the approved design, one vertical slice at a time.

- **Scope:** For each slice, read the affected requirements and code, keep domain logic independent from transport, represent business values as structured data where required, add unit/integration tests with the behavior, and update plan progress. Preserve challenge semantics and avoid unrelated refactoring or speculative infrastructure.

- **Expected artifacts:** Production code, tests, configuration/rule data, documentation or ADR updates when justified, and updated plan checkboxes.

- **Deterministic quality gate:** Each completed slice has focused tests and the repository verification command passes at the milestone; the slice has no known intentional requirement gap or unresolved architectural ambiguity.

- **Failure routing:** Test or implementation failure returns to the Implementer for correction. Requirement, architecture, or challenge contract conflict returns to Phase 1/Architect; the Implementer must not silently rewrite the plan or invent behavior.

- **AI Journey Capture rule:** Capture only material decisions, edge-case discoveries, failed approaches, or corrections. Routine implementation and boilerplate do not create entries; otherwise report `NO_JOURNEY_ENTRY`.

- **Next phase:** Repeat Phase 2 for the next approved slice; when all planned slices are complete, continue to Phase 3.

---

### Phase 3 — Deterministic verification and evidence consolidation

- **Agent:** Implementer (`.agents/agents/implementer.md`)
- **Skill:** implement (`.agents/skills/implement/SKILL.md`)
- **Context:** continue
- **Objective:** Produce objective verification evidence before any independent review.

- **Scope:** Run the complete repository verification command from a clean state as applicable; confirm unit and integration coverage for classification boundaries, keyword priority/case handling, formula/penalty/cap/rounding, income lookup, denial, validation, and the API contract. Compare all provided official samples exactly when the official artifact is present. Record requirement-to-evidence links without declaring independent approval.

- **Expected artifacts:** Verification output, test results, sample comparison evidence when available, and updated requirement traceability.

- **Deterministic quality gate:** The single-command test suite and relevant repository checks pass; evidence covers every implemented material requirement; absent official artifacts are marked unavailable, never fabricated.

- **Failure routing:** Implementation or test failures return to Phase 2. Missing evidence returns to the Implementer for tests or documentation. Missing/ambiguous challenge inputs route to the Architect/human and block claims that depend on them.

- **AI Journey Capture rule:** Capture only verification-driven discoveries or corrections that materially changed implementation or tests. Otherwise report `NO_JOURNEY_ENTRY`.

- **Next phase:** Phase 4 only after the deterministic gate passes.

---

### Phase 4 — Independent technical review

- **Agent:** Reviewer (`.agents/agents/reviewer.md`)
- **Skill:** review (`.agents/skills/review/SKILL.md`)
- **Context:** fresh when practical, with access to the repository and Phase 3 evidence
- **Objective:** Independently find correctness, requirement, edge-case, architecture, testing, and unnecessary-complexity problems.

- **Scope:** Inspect the actual diff, source, tests, plan, requirements, ADRs, and verification output. Review priority semantics, threshold boundaries, deterministic financial behavior, configuration duplication, domain/API separation, validation, and test strength. Produce findings before advice.

- **Expected artifacts:** Review findings ordered by severity, verification commands/results, and exactly one decision: `PASS` or `CHANGES_REQUIRED`.

- **Deterministic quality gate:** Review is evidence-based and complete; the Reviewer does not modify the reviewed implementation or silently fix it.

- **Failure routing:** `CHANGES_REQUIRED` routes findings to Phase 5. A requirement or architecture conflict routes to the Architect; review itself never weakens the requirement or approves on test passage alone.

- **AI Journey Capture rule:** Record only review findings that changed the design, implementation, tests, or scope. Otherwise report `NO_JOURNEY_ENTRY`.

- **Next phase:** Phase 5 for `CHANGES_REQUIRED`; Phase 6 for `PASS`.

---

### Phase 5 — Review corrections and deterministic re-verification

- **Agent:** Implementer (`.agents/agents/implementer.md`)
- **Skill:** implement (`.agents/skills/implement/SKILL.md`)
- **Context:** continue with the Reviewer findings
- **Objective:** Resolve accepted material findings without allowing review to become an untracked redesign.

- **Scope:** Apply only justified corrections, add or strengthen tests, update the approved plan/traceability when necessary, and rerun the complete deterministic verification. Re-enter Phase 4 with fresh review context after the gate passes.

- **Expected artifacts:** Corrective diff, tests, updated evidence, and a disposition for each finding.

- **Deterministic quality gate:** All accepted findings are resolved or explicitly dispositioned by the responsible authority, tests and repository checks pass, and no new known requirement gap is introduced.

- **Failure routing:** New ambiguity or scope conflict routes to Phase 1. Failed verification returns to the Implementer within this phase/Phase 2 as appropriate. Do not proceed to audit while material findings remain open.

- **AI Journey Capture rule:** Record meaningful review-driven corrections or rejected complexity only; otherwise report `NO_JOURNEY_ENTRY`.

- **Next phase:** Phase 4 for independent re-review, then Phase 6 only after the Reviewer returns `PASS`.

---

### Phase 6 — Final submission Challenge Audit

- **Agent:** Challenge Auditor (`.agents/agents/challenge-auditor.md`)
- **Skill:** challenge-audit (`.agents/skills/challenge-audit/SKILL.md`)
- **Context:** fresh from implementation and prior audits
- **Objective:** Determine submission compliance as an evaluator, not merely whether the code appears technically sound.

- **Scope:** Inspect the full requirements matrix, source, tests, architecture documentation, ADRs, AI Journey, and verification results. Map every material requirement to concrete evidence and run the complete suite. Verify exact official samples only if supplied. Do not implement missing behavior or grant credit without evidence.

- **Expected artifacts:** The required requirement/status/evidence table, blocking gaps, non-blocking improvements, and exactly one final assessment: `READY_FOR_SUBMISSION` or `NOT_READY_FOR_SUBMISSION`.

- **Deterministic quality gate:** Every material requirement is `VERIFIED`, required tests and exact sample comparisons pass when available, and no blocking gap remains. Missing official artifacts remain explicitly `MISSING` or `AMBIGUOUS` and prevent readiness where they are required for proof.

- **Failure routing:** `NOT_READY_FOR_SUBMISSION` routes blocking gaps to the Architect for interpretation or the Implementer for approved remediation; remediation must pass Phase 3 and Phase 4 again before a fresh final audit. The Auditor does not implement fixes.

- **AI Journey Capture rule:** Capture only audit findings that materially changed remediation, evidence, or submission scope. Otherwise report `NO_JOURNEY_ENTRY`.

- **Next phase:** Submission only after `READY_FOR_SUBMISSION`; otherwise the routed remediation cycle returns through the applicable earlier phase and ends with a new Phase 6 audit.

---

## Operating Constraints

- This workflow references existing role and skill contracts; it does not duplicate them or create one skill per phase.
- It creates no runtime, database, DAG engine, orchestration code, or vendor-specific dependency.
- The Credit Engine remains the subject of the work; process exists only to produce concrete requirement and verification evidence.
- Challenge Auditor runs at submission readiness, or when explicitly requested/significant compliance risk exists, not for every small change.
- No phase may silently assume authority owned by another role.
- No phase may fabricate unavailable challenge artifacts.
- No phase may advance through an unresolved human-decision blocker.
- A phase completes only when its quality gate passes.
- The workflow terminates only when Phase 6 returns `READY_FOR_SUBMISSION`.