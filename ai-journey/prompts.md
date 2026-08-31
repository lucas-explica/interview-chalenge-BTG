# Selected AI Interactions

This is intentionally not a complete conversation log. Only interactions
that materially influenced the engineering solution are documented here.

---

## Interaction: COO and Coordinator keyword ambiguity

**Tool:** Codex.

**Original prompt:** The original prompt text is not retained in repository
history. This entry records only the verifiable discovery and its effect on
the implementation.

**Initial AI response/approach:** The original response is not retained. The
verifiable approach in the resulting implementation used the challenge's
ordered keyword lists and case-insensitive substring matching, with the first
matching category winning.

**Critical evaluation:** `COO` is a substring of `Coordinator`. Therefore a
title containing `Coordinator` matches `EXECUTIVE` before
`SENIOR_PROFESSIONAL`, even though “Coordinator” is also explicitly listed as
a senior keyword. This is an observable ambiguity in the supplied rules, not a
fact that can be silently normalized by the implementation.

**Iteration/correction:** No behavior was changed because resolving this
challenge ambiguity requires an authoritative product decision. The ambiguity
was retained as an explicit review item; the existing top-down substring rule
remains unchanged.

**Final decision:** Preserve the challenge's stated top-down,
case-insensitive, anywhere-in-title matching semantics and surface the
COO/Coordinator overlap rather than inventing a token-boundary rule.

**Evidence:** `src/Btg.CreditEngine.Domain/JobRules.cs` contains both keywords
and `JobClassifier` implements ordered substring matching. The executable
decision is covered by `JobClassificationTests.CoordinatorFollowsLiteralSubstringPriority`.

---

## Interaction: unsupported debt consistency validation

**Tool:** Codex.

**Observation:** External staff-level review identified that the validator
had imposed consistency between `has_market_debt` and `market_debt_types`,
including a uniqueness requirement, although the challenge defines the fields
separately and consumes them independently in different rules.

**Challenge evidence:** The cluster A rule reads `has_market_debt`, while the
cluster B exclusion and default-debt penalty read `market_debt_types`.

**Implementation conflict:** The extra validator errors rejected combinations
that the challenge does not prohibit.

**Correction:** Removed only the unsupported consistency and duplicate-entry
rejections; required fields, declared values, score range, region, and debt
type validation remain.

**Regression evidence:** `CustomerValidationTests` proves both
`has_market_debt=false` with `mortgage` and `has_market_debt=true` with an
empty list are valid, while cluster evaluation remains covered separately.

---

## Interaction: default-debt penalty and cluster priority

**Tool:** Codex.

**Original prompt:** The original prompt text is not retained in repository
history. The recorded interaction is preserved here as a concise excerpt of
the decision that was made.

**Initial AI response/approach:** The original response is not retained. The
recorded initial expectation placed a defaulted customer in `CLUSTER_B`.

**Critical evaluation:** The cluster table explicitly excludes `credit_default`
and `loan_default` from `CLUSTER_B`, so the first matching cluster is
`CLUSTER_C`. The penalty must then be applied to the resulting cluster and job
multiplier, before the cap.

**Iteration/correction:** The expectation was corrected and a regression test
was added for the defaulted customer.

**Final decision:** Preserve cluster priority, classify the customer as
`CLUSTER_C`, then apply the default-debt penalty. The resulting senior limit is
R$ 3,800 after deterministic rounding.

**Evidence:** `src/Btg.CreditEngine.Domain/ClusterRules.cs`,
`src/Btg.CreditEngine.Domain/FinancialRules.cs`, and
`tests/Btg.CreditEngine.Domain.Tests/FinancialCalculationTests.cs`; commit
`5bbe171` records the implementation and test change.
