# BTG Credit Engine

This repository contains the BTG Pactual Credit Engine REST API. The
implementation is being delivered in small phases defined in
`.agents/plans/btg-credit-engine-implementation.md`.

## Phase 1

The solution currently contains the ASP.NET Core API host, an HTTP-independent
domain project, and unit/integration test projects. Business behavior is not
implemented in this bootstrap phase. The API exposes only `/health` so the
integration test can prove that the host starts.

## Customer validation

`POST /customers/classify` now accepts the documented customer shape using
`snake_case` JSON names. This phase validates required fields, score range,
Brazilian region and debt-type vocabulary, state abbreviation shape, and
consistency between `has_market_debt` and `market_debt_types`. A valid request
is echoed unchanged until the classification phases add enriched fields.

Invalid JSON or input returns HTTP 400 with this stable shape:

```json
{
  "error": "validation_error",
  "errors": [{ "field": "score", "message": "Score must be between 0 and 1000." }]
}
```

For a valid request, the response preserves all input fields and adds
`cluster_id`, `cluster_name`, `job_category`, `monthly_income`, `approved`,
and `approved_limit`. The current deterministic example is:

```json
{
  "id": "customer-1",
  "name": "Ana Silva",
  "age": 30,
  "score": 700,
  "has_market_debt": false,
  "market_debt_types": [],
  "location": { "city": "São Paulo", "state": "SP", "region": "Sudeste" },
  "job_title": "Engineer",
  "cluster_id": "CLUSTER_A",
  "cluster_name": "Diamond",
  "job_category": "MID_PROFESSIONAL",
  "monthly_income": 12000,
  "approved": true,
  "approved_limit": 50000
}
```

## Verification

Run the complete deterministic build and test suite from the repository root:

```text
dotnet test Btg.CreditEngine.sln --configuration Release
```
