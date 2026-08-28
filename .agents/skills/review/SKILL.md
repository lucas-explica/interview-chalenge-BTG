# Review

## Purpose

Perform an independent engineering review of a completed change.

## Procedure

1. Read `docs/requirements.md`.
2. Read relevant ADRs.
3. Read the implementation plan.
4. Inspect the actual diff.
5. Inspect relevant surrounding code.
6. Inspect tests.
7. Run verification.
8. Look specifically for:
   - incorrect priority handling
   - threshold mistakes
   - rounding mistakes
   - configuration duplicated in code
   - missing validation
   - weak tests
   - architecture leakage
   - unnecessary abstractions
9. Produce findings before suggesting improvements.

## Severity

### BLOCKER

Submission should not proceed.

### HIGH

Likely correctness, requirement, or significant design problem.

### MEDIUM

Maintainability, test weakness, or meaningful design issue.

### LOW

Minor improvement.

## Decision

End with:

PASS

or

CHANGES_REQUIRED