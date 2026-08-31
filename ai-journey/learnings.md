# AI Collaboration Learnings

## Rule interaction

Priority rules must be tested together, not only in isolation. A default-debt
penalty cannot affect an approved A or B customer under the supplied tables:
cluster A requires no market debt and cluster B excludes default debt. This
interaction was verified and captured in the financial regression test.
