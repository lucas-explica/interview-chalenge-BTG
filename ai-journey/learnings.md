# AI Collaboration Learnings

## What Worked Well

_To be completed during development._

Examples of useful areas to observe:

- requirement decomposition
- architecture exploration
- generation of repetitive test cases
- adversarial review
- boundary-condition discovery

---

## What Did Not Work Well

_To be completed with real examples._

Do not manufacture AI failures for the document.

Record cases where AI:

- misunderstood a requirement
- proposed unnecessary complexity
- missed a boundary condition
- duplicated business rules in code
- produced an incorrect calculation
- created a weak test
- made an unjustified architectural assumption

---

## Corrections That Improved the Solution

For meaningful mistakes, record:

### Situation

What AI proposed.

### Problem

Why the proposal was wrong or weaker.

### Correction

How the design or implementation changed.

### Lesson

What this says about effective AI-assisted engineering.

---

## What I Would Do Differently

_To be completed near submission._

Consider:

- prompts that should have included stronger constraints
- where independent review was most valuable
- where deterministic tools were more appropriate than AI
- which parts of the harness helped
- which parts were unnecessary
## Rule interaction

Priority rules must be tested together, not only in isolation. A default-debt
penalty cannot affect an approved A or B customer under the supplied tables:
cluster A requires no market debt and cluster B excludes default debt. This
interaction was verified and captured in the financial regression test.
