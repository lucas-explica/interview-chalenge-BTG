# Selected AI Interactions

This is intentionally not a complete conversation log.

Only interactions that materially influenced the engineering solution
are documented here.

---

## Prompt: <decision or problem>

**Tool:**

<tool used>

**Context:**

What problem were we solving?

**What I asked:**

> <important part of the prompt>

**Initial AI suggestion:**

Summarize the proposed approach.

**Critical evaluation:**

What was correct?

What was incomplete, risky, or incorrect?

**Iteration:**

What changed after challenging or refining the result?

**Final decision:**

Describe the approach actually adopted.

**Evidence:**

- ADR:
- implementation:
- tests:

---

## Prompt: <next important interaction>

Repeat the same structure.
## Decision: default-debt penalty interaction

**What happened:** While adding financial tests, an initial expectation placed
a defaulted customer in `CLUSTER_B`. The cluster rule explicitly excludes
`credit_default` and `loan_default`, so the actual first matching cluster is
`CLUSTER_C`.

**Final solution:** The implementation preserves cluster priority and applies
the penalty only after the resulting cluster and job multiplier are known.
The regression test verifies `CLUSTER_C` senior income and a rounded limit of
R$ 3,800 for a defaulted customer.
