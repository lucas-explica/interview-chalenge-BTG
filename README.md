# BTG Credit Engine

ASP.NET Core REST API that classifies customers, estimates monthly income, and
calculates deterministic credit limits. Domain rules are independent from the
HTTP adapter and represented as structured rule data.

## Prerequisites and setup

- .NET 10 SDK
- A shell capable of running the commands below

From the repository root, restore and build the solution:

```text
dotnet restore Btg.CreditEngine.sln
dotnet build Btg.CreditEngine.sln --configuration Release
```

## Run the API

```text
dotnet run --project src/Btg.CreditEngine.Api --launch-profile http
```

The HTTP profile listens at `http://localhost:5231`. Stop the process with
`Ctrl+C` when finished.

## Classify a customer

Call `POST http://localhost:5231/customers/classify` with a JSON customer using
the documented `snake_case` field names. For example:

```bash
curl -X POST http://localhost:5231/customers/classify \
  -H "Content-Type: application/json" \
  -d '{"id":"customer-1","name":"Ana Silva","age":30,"score":700,"has_market_debt":false,"market_debt_types":[],"location":{"city":"São Paulo","state":"SP","region":"Sudeste"},"job_title":"Engineer"}'
```

PowerShell equivalent:

```powershell
$body = '{"id":"customer-1","name":"Ana Silva","age":30,"score":700,"has_market_debt":false,"market_debt_types":[],"location":{"city":"São Paulo","state":"SP","region":"Sudeste"},"job_title":"Engineer"}'
Invoke-RestMethod -Method Post -Uri http://localhost:5231/customers/classify -ContentType 'application/json' -Body $body
```

The response preserves the input and adds `cluster_id`, `cluster_name`,
`job_category`, `monthly_income`, `approved`, and `approved_limit`.

Invalid JSON or input returns HTTP 400 with this stable shape:

```json
{
  "error": "validation_error",
  "errors": [{ "field": "score", "message": "Score must be between 0 and 1000." }]
}
```

See [docs/architecture.md](docs/architecture.md) for the implemented
architecture and its trade-offs.

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

Run the complete test suite from the repository root with one command:

```text
dotnet test Btg.CreditEngine.sln --configuration Release
```
