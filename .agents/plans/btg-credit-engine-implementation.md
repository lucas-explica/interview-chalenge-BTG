# Plan: BTG Credit Engine Implementation

## Goal

Take the repository from its current documentation-only state to a small,
submission-ready .NET/C# ASP.NET Core REST API that classifies a customer,
calculates monthly income and a deterministic credit limit, and proves the
challenge behavior with unit and integration tests.

This plan is based on docs/requirements.md, the repository agent contracts,
and .agents/workflows/btg-credit-engine.md. There are currently no accepted
ADRs, source projects, tests, official sample customers, or
expected-output.json in the repository.

## Requirements

The challenge does not provide stable IDs, so this plan assigns local IDs for
traceability. They should be carried into implementation evidence without
changing the challenge text.

- **REQ-001 — Technology and stateless API:** use .NET, C#, ASP.NET Core;
  expose POST /customers/classify; do not persist requests between calls.
- **REQ-002 — Customer contract:** accept the documented customer fields and
  nested location fields, including the documented debt-type vocabulary.
- **REQ-003 — Cluster classification:** evaluate CLUSTER_A through D in
  priority order, with inclusive score/age boundaries and the exact debt
  conditions from the challenge.
- **REQ-004 — Job classification:** perform case-insensitive substring matching
  anywhere in job_title, evaluate categories top-down, and stop at the first
  match.
- **REQ-005 — Monthly income:** deterministically look up the value for every
  cluster × job-category combination, including zero income for CLUSTER_D.
- **REQ-006 — Credit calculation:** apply base limit × job multiplier ×
  penalty, enforce the cluster cap, round to the nearest R$100, and force
  CLUSTER_D to an unapproved zero limit.
- **REQ-007 — Penalty:** apply DEFAULT_DEBT_PENALTY after the job multiplier
  and before the cluster cap when either default debt type is present.
- **REQ-008 — Validation and errors:** reject invalid or missing input with an
  appropriate client error and a stable, documented response shape.
- **REQ-009 — Unit evidence:** cover cluster paths and boundaries, job keyword
  case/priority behavior, income lookup, formula, penalty, cap, rounding, and
  denial behavior in isolation.
- **REQ-010 — Integration evidence:** exercise the full valid and invalid REST
  request/response cycle and verify the complete output contract.
- **REQ-011 — Official samples:** when the official six customers and
  expected-output artifact are available, compare all six outputs exactly.
  Until then this is an explicit external gap; no substitute expected output
  may be fabricated.
- **REQ-012 — Verification:** provide one repository command that runs the
  complete deterministic test/check suite.
- **REQ-013 — Documentation and AI Journey:** document API usage, architecture
  and meaningful AI-assisted decisions in the existing ai-journey/ files.
- **REQ-014 — Submission review:** complete independent technical review and a
  fresh challenge requirement-to-evidence audit before submission.

## Design

Use one small solution with a production ASP.NET Core API project, a domain
library/project, and focused unit/integration test projects. The domain owns
the customer value objects, validation result, cluster/job classification,
income lookup, penalty, and credit calculation. It receives structured rule
data and has no HTTP, controller, JSON, or ASP.NET Core dependency.

Represent the challenge tables as explicit structured rule data: ordered
cluster rules, ordered job categories, income matrix, penalties, base limits,
caps, and multipliers. The evaluator code only interprets these records. Keep
the default data in the repository and fail fast if it is malformed; do not
add a database or generic rules engine.

The API adapter maps JSON to the input model, invokes one stateless domain
service, and maps the result to the required enriched response. Use ASP.NET
Core's normal model-validation/error mechanism with documented HTTP 400
behavior unless an official challenge artifact later specifies a different
error contract. Keep JSON naming aligned with the snake_case challenge
contract, including has_market_debt, market_debt_types, and the location
object.

The implementation must preserve this ordering: cluster rules are first-match
priority order; job keywords are first-match top-down; the job multiplier
precedes the penalty; the penalty precedes the cap; CLUSTER_D is always
denied. Use decimal-based calculation and an explicit nearest-R$100 rounding
helper. The exact tie behavior of “nearest” is not stated by the challenge;
if a tie case is required, surface it for human/architect decision and
document the chosen semantics rather than silently changing the contract.

## Files Likely Affected

- .agents/plans/btg-credit-engine-implementation.md
- src/ or an equivalently minimal .NET solution layout
- tests/ or the selected unit/integration test layout
- checked-in rule configuration/data files
- docs/requirements.md (only to add evidence links/status after behavior
  exists)
- docs/adr/ (only for meaningful decisions such as rule-data format or
  rounding/validation semantics)
- README.md (API, setup, and one-command verification documentation)
- ai-journey/prompts.md
- ai-journey/learnings.md
- repository verification configuration/scripts, if needed

## Phases

### Phase 1 — Bootstrap the minimal .NET solution and verification command

**Goal:** create a buildable, testable application skeleton without adding
business behavior that belongs in later slices.

**Requirements:** REQ-001, REQ-012.

- [x] Create the smallest .NET solution with an ASP.NET Core API, a domain
      project, unit tests, and integration tests; confirm the selected target
      framework is supported by the available SDK.
- [x] Establish project references so domain code is independent of HTTP and
      infrastructure.
- [x] Add the repository's single verification command (build plus all tests,
      and any required encoding/documentation checks) and document it.
- [x] Add a minimal health/startup path only if useful for integration hosting;
      do not add persistence, messaging, or unrelated endpoints.
- [x] Tests: prove the solution builds, test discovery works, and the API test
      host can start.

**Dependencies:** none.

**Observable behavior:** a clean checkout restores, builds, and runs the
empty test suite through one documented command; no production business rule
is claimed complete.

**Gate:** the verification command passes and the dependency graph shows no
domain-to-ASP.NET Core reference.

### Phase 2 — Customer contract and deterministic validation slice

**Goal:** accept a valid customer shape and reject malformed input before
classification.

**Requirements:** REQ-001, REQ-002, REQ-008, REQ-010.

- [x] Define domain input types for all required customer and location fields,
      plus the documented debt-type values.
- [x] Define validation rules justified by the schema and challenge (required
      fields, score range, valid region/debt values); validate
      `has_market_debt` and `market_debt_types` independently and do not invent
      consistency or uniqueness restrictions.
- [x] Add API JSON mapping with the exact external field names and a documented
      400 error response.
- [x] Tests: domain validation for missing, malformed, out-of-range, and valid
      inputs; integration tests for valid deserialization and invalid/missing
      fields with stable client errors.

**Dependencies:** Phase 1.

**Observable behavior:** invalid input never reaches business evaluation;
valid input can traverse the API boundary into the domain.

**Gate:** focused unit and integration tests pass, and the serialized contract
is verified for every input field including nested location data.

### Phase 3 — Data-driven cluster classification slice

**Goal:** classify a validated customer into the first matching cluster using
explicit ordered rule data.

**Requirements:** REQ-003, REQ-002, REQ-009.

- [x] Add ordered cluster rule records containing IDs, names, score/age
      predicates, debt conditions, base limits, and caps.
- [x] Implement a first-match evaluator; keep rule values out of nested
      conditionals and preserve CLUSTER_D as the final catch-all.
- [x] Cover CLUSTER_A, B, C, and D, including score 700/500/300 boundaries,
      age 18/25/60/65 boundaries, no-debt behavior, default-debt exclusions,
      and priority overlaps.
- [x] Add a focused configuration/data integrity test for rule ordering,
      unique IDs, and required values.

**Dependencies:** Phase 2.

**Observable behavior:** every valid customer receives exactly one cluster;
overlapping conditions select the highest-priority matching rule.

**Gate:** isolated classifier tests and data integrity tests pass, including
boundary and overlap cases; no HTTP types appear in the domain project.

### Phase 4 — Job-title classification and priority slice

**Goal:** determine the first matching job category and multiplier.

**Requirements:** REQ-004, REQ-009.

- [x] Add ordered category records with category IDs, multipliers, and the
      exact keyword lists from the challenge.
- [x] Implement case-insensitive substring matching anywhere in the title and
      return the first category match; use OTHER when no keyword matches.
- [x] Tests: one path per category, no-match fallback, mixed case, keyword in
      the middle of a title, executive-vs-senior overlap, and any other
      priority overlap exposed by the keyword lists.

**Dependencies:** Phase 3 for shared rule-data conventions, but it can be
reviewed independently at the domain boundary.

**Observable behavior:** a title containing both a higher- and lower-priority
keyword always selects the higher-priority category.

**Gate:** category and priority tests pass without duplicating the keyword
table in evaluator branches.

### Phase 5 — Income, penalty, limit, cap, rounding, and denial slice

**Goal:** produce the deterministic financial result from the cluster and job
classification.

**Requirements:** REQ-005, REQ-006, REQ-007, REQ-009.

- [x] Add the complete cluster × job-category monthly income matrix as
      structured data and test every combination, including all CLUSTER_D
      zeros.
- [x] Add the penalty rule data and apply the default-debt penalty after the
      job multiplier and before cap enforcement.
- [x] Implement the formula with decimal arithmetic, cap enforcement, the
      explicit nearest-R$100 helper, and CLUSTER_D's forced
      approved=false, approved_limit=0 result.
- [x] Test base calculations, penalty trigger/non-trigger, cap below the
      uncapped value, values already on a hundred, values on both sides of a
      hundred boundary, and denial. Add a documented decision/ADR before
      testing exact half-hundred ties if the challenge owner confirms a tie
      convention is required.
- [x] Verify that income is a lookup result and is not accidentally recomputed
      from the credit limit.

**Dependencies:** Phases 3 and 4.

**Observable behavior:** the domain returns a complete deterministic result
for every cluster/category combination and never approves CLUSTER_D.

**Gate:** all financial unit tests pass with decimal precision and explicit
rounding semantics; the formula order is demonstrated by a penalty-plus-cap
case.

### Phase 6 — Complete REST response contract slice

**Goal:** expose the full enriched classification result through the required
endpoint.

**Requirements:** REQ-001, REQ-002, REQ-005, REQ-006, REQ-007, REQ-008,
REQ-010.

- [x] Add POST /customers/classify and map the validated request to the domain
      service exactly once.
- [x] Define the enriched response fields from the challenge output contract;
      preserve input fields and use the required names/types for cluster, job
      category, income, approval, limit, and any other documented outputs.
- [x] Add integration tests for a representative approved result, a penalized
      result, a cap-limited result, and CLUSTER_D denial; assert status,
      response shape, values, and no state carried between requests.
- [x] Update API documentation with request/response examples generated from
      implemented behavior, not fabricated official samples.

**Dependencies:** Phases 2–5.

**Observable behavior:** a valid POST returns the same customer enriched with
all calculated fields; invalid requests return the documented client error.

**Gate:** full request/response integration tests pass and no controller/API
code contains business-rule calculations.

### Phase 7 — Official sample comparison and evidence consolidation

**Goal:** prove the implementation against the challenge's exact examples and
make every requirement traceable to evidence.

**Requirements:** REQ-009, REQ-010, REQ-011, REQ-012, REQ-013.

- [x] If the official six sample customers and expected-output.json become
      available, add them as test fixtures and compare the complete serialized
      outputs exactly, including numeric formatting and field presence. (The
      artifacts remain unavailable in this repository.)
- [x] If they remain unavailable, record the external gap prominently in the
      requirements/evidence documentation and report that REQ-011 cannot be
      verified; do not create replacement expected outputs or claim readiness
      on that basis.
- [x] Run the complete one-command verification from a clean repository state,
      including encoding checks required by AGENTS.md where applicable.
- [x] Update docs/requirements.md with implementation/test/configuration
      evidence links and statuses; do not mark evidence as verified before it
      exists.
- [x] Complete API/project documentation and record only real, material AI
      decisions, corrections, and lessons in ai-journey/.

**Dependencies:** Phase 6; official artifact availability is external and not
a prerequisite for implementing local behavior, but it is a prerequisite for
claiming exact-sample verification.

**Observable behavior:** one command produces reproducible pass/fail evidence
for all local tests, and the evidence map distinguishes verified requirements
from the unavailable sample gap.

**Gate:** local deterministic verification is green; all evidence links are
concrete; the sample comparison is either passing or explicitly unavailable.

### Phase 8 — Independent review, remediation, and final challenge audit

**Goal:** reach submission readiness through the repository's mandated role
boundaries.

**Requirements:** REQ-013, REQ-014 and all prior requirements.

- [x] Have a fresh-context Reviewer inspect requirements, ADRs, plan, source,
      tests, configuration, and verification output; require findings ordered
      by severity and an exact PASS or CHANGES_REQUIRED decision.
- [ ] If changes are required, implement only accepted findings, add/regress
      tests, rerun complete verification, and obtain a fresh re-review.
- [ ] Run a fresh Challenge Auditor after technical review; require its
      requirement/status/evidence table, blocking gaps, non-blocking
      improvements, and exact READY_FOR_SUBMISSION or
      NOT_READY_FOR_SUBMISSION conclusion.
- [ ] Resolve every blocking finding. If official samples remain unavailable,
      disposition the gap according to challenge-owner guidance; do not label
      the submission ready while a required exact-output proof remains
      unverified.
- [ ] Perform the final AI Journey review so entries are honest, material, and
      contain no invented failures, prompts, or evidence.

**Dependencies:** Phase 7 and a passing deterministic gate.

**Observable behavior:** independent review and audit artifacts exist, all
material requirements have concrete evidence or an explicit blocking gap, and
the final audit alone determines submission readiness.

**Gate:** Reviewer returns PASS, the final Challenge Auditor returns
READY_FOR_SUBMISSION, and the clean-state one-command verification remains
green.

## Risks

- **Official sample artifacts are absent.** Keep REQ-011 explicitly blocked or
  partial until the real six inputs and expected output are supplied; never
  fabricate fixtures.
- **Nearest-R$100 tie behavior is unspecified.** Use an explicit decimal
  helper, surface exact half cases for human/architect decision, and record
  the decision if such cases are part of the accepted contract.
- **Validation error shape is underspecified.** Document the chosen standard
  ASP.NET Core 400 response and revise only if the original challenge artifact
  supplies an exact contract.
- **Rule values drift from evaluator code.** Keep values in one checked-in
  structured source and add integrity/behavior tests that exercise it.
- **Threshold and priority regressions.** Use explicit boundary and overlap
  tests before relying on sample tests.
- **Encoding or platform drift.** Save all code/documentation as UTF-8 without
  BOM and LF; run the required encoding check before frontend changes (none
  are currently present) and the repository verification command before each
  milestone.

## Done When

- The minimal .NET/C#/ASP.NET Core solution builds and runs statelessly.
- Customer validation, cluster priority, job keyword priority, income lookup,
  penalty, cap, rounding, and CLUSTER_D denial match the challenge rules.
- Rules are represented as structured data and domain behavior is independent
  from HTTP and serialization.
- Unit and integration tests cover all required paths and boundaries.
- POST /customers/classify returns the documented enriched contract and
  appropriate validation errors.
- A single repository command provides reproducible deterministic verification.
- Official six-customer exact comparison is passing if the real artifacts are
  available; otherwise the external gap is clearly documented and not claimed
  as verified.
- API/project documentation, meaningful AI Journey entries, ADRs for any
  material decisions, and requirement-to-evidence traceability are complete.
- Fresh independent review returns PASS and the final Challenge Auditor
  returns READY_FOR_SUBMISSION.
