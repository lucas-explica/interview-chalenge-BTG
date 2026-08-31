# AI Collaboration Learnings

## What worked well

- Using AI to challenge rule interactions exposed that a default-debt penalty
  cannot make a customer qualify for `CLUSTER_B`; the cluster must be selected
  first.
- Deterministic unit tests made the correction concrete and prevented the
  interaction from remaining only a documentation claim.

## What didn't work

- The initial default-debt expectation was wrong because it overlooked the
  `CLUSTER_B` default-debt exclusion.
- The original prompt text for the recorded interactions was not retained,
  which limits how precisely the AI Journey can reproduce the conversation.
- The explicit `COO`/`Coordinator` overlap is easy to miss when reviewing
  keyword lists without checking substring behavior.

## What I'd do differently

- Preserve short, truthful excerpts of every material prompt and response
  while the work is happening.
- Test overlapping keywords, including `COO`/`Coordinator`, before treating
  the job-category table as complete.
- Ask for priority, boundary, and ambiguity cases explicitly before accepting
  an implementation approach.

## Reusable lesson

Priority rules must be tested together, not only in isolation. A default-debt
penalty cannot affect an approved A or B customer under the supplied tables:
cluster A requires no market debt and cluster B excludes default debt. This
interaction was verified and captured in the financial regression test.
