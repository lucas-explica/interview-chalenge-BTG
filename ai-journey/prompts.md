# Selected AI Interactions

This is intentionally not a complete conversation log.

Only interactions that materially influenced the engineering solution
are documented here.

---

## Decision: default-debt penalty interaction

**What happened:** While adding financial tests, an initial expectation placed
a defaulted customer in `CLUSTER_B`. The cluster rule explicitly excludes
`credit_default` and `loan_default`, so the actual first matching cluster is
`CLUSTER_C`.

**Final solution:** The implementation preserves cluster priority and applies
the penalty only after the resulting cluster and job multiplier are known.
The regression test verifies `CLUSTER_C` senior income and a rounded limit of
R$ 3,800 for a defaulted customer.

**Evidence:** `src/Btg.CreditEngine.Domain/ClusterRules.cs`,
`src/Btg.CreditEngine.Domain/FinancialRules.cs`, and
`tests/Btg.CreditEngine.Domain.Tests/FinancialCalculationTests.cs`.
