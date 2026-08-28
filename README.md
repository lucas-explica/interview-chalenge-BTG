# BTG Credit Engine

This repository contains the BTG Pactual Credit Engine REST API. The
implementation is being delivered in small phases defined in
`.agents/plans/btg-credit-engine-implementation.md`.

## Phase 1

The solution currently contains the ASP.NET Core API host, an HTTP-independent
domain project, and unit/integration test projects. Business behavior is not
implemented in this bootstrap phase. The API exposes only `/health` so the
integration test can prove that the host starts.

## Verification

Run the complete deterministic build and test suite from the repository root:

```text
dotnet test Btg.CreditEngine.sln --configuration Release
```
