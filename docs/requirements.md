# **BTG Pactual Credit Engine — Backend Challenge** 

Build a REST API that classifies customers into risk clusters, calculates personalised credit limits, and estimates monthly income. The classification engine must be data-driven — the business rules defined below should be representable as configuration, not buried in logic. 

<mark>🤖</mark> **<mark>AI is welcome here.</mark>** <mark>We want to know</mark> _<mark>how</mark>_ <mark>you used it, not</mark> _<mark>if</mark>_ <mark>you did.</mark> 

## **Input Data** 

### **<mark>Customer Schema</mark>** 

|**Field**|**Type**|**Description**|
|---|---|---|
|`id`|string|Unique identifier|
|`name`|string|Full name|
|`age`|integer|Age in years|
|`score`|integer|Credit bureau score (0–1000)|
|`has_market_debt`|boolean|Whether the customer has any recorded market debt|
|`market_debt_types`|string[]|Active debt types (see below)|
|`location.city`|string|City of residence|
|`location.state`|string|State abbreviation (e.g.<br>`SP` ,<br>`RJ` )|
|`location.region`|string|`Norte`·<br>`Nordeste`·<br>`Centro-Oeste`·<br>`Sudeste`·<br>`Sul`|
|`job_title`|string|Free-text job title|



**<mark>Valid</mark>** **`market_debt_types`** **<mark>values:</mark>** 

|**Value**|**Meaning**|
|---|---|
|`credit_card`|Active credit card (non-defaulted)|
|`personal_loan`|Active personal loan (non-defaulted)|
|`mortgage`|Mortgage (non-defaulted)|
|`credit_default`|Credit card in collections⚠|
|`loan_default`|Loan in collections⚠|



## **Business Rules** 

**Design note:** The tables in this section define what the system must enforce. How you represent and load these rules is your design decision — be prepared to justify it. 

### **<mark>1. Customer Clusters</mark>** 

<mark>Evaluate in priority order. Assign the</mark> **<mark>first</mark>** <mark>cluster whose conditions are fully met.</mark> 

|**Priority**|**Cluster ID**|**Name**|**Score**|**Age**|**Debt Condition**|**Base**<br>**Limit**|**Cap**|
|---|---|---|---|---|---|---|---|
|1|`CLUSTER_A`|Diamond|≥<br>700|25–<br>60|`has_market_debt == false`|R$ 50,000|R$ 100,000|
|2|`CLUSTER_B`|Gold|≥<br>500|18–<br>65|no<br>`credit_default`or<br>`loan_default`in<br>`market_debt_types`|R$ 20,000|R$ 40,000|
|3|`CLUSTER_C`|Silver|≥<br>300|—|—|R$ 5,000|R$ 10,000|



|**Priority**|**Cluster ID**|**Name**|**Score**|**Age**||**Debt Condition**|**Base**<br>**Limit**|**Cap**|
|---|---|---|---|---|---|---|---|---|
|4|`CLUSTER_D`|Bronze|—|—|catch-all||R$ 0|R$ 0 —<br>denied|



### **<mark>2. Job Title Categories</mark>** 

<mark>Match case-insensitively anywhere in the job title. Evaluate top-down; first match wins.</mark> 

|**Priority**|**Category**|**Multiplier**|**Keywords**|
|---|---|---|---|
|1|`EXECUTIVE`|×2.0|CEO, CFO, CTO, COO, CIO, CMO, Chief, President, Vice President, VP, Director|
|2|`SENIOR_PROFESSIONAL`|×1.5|Senior, Lead, Manager, Coordinator, Supervisor, Principal|
|3|`MID_PROFESSIONAL`|×1.0|Engineer, Analyst, Developer, Specialist, Designer, Accountant, Consultant, Architect|
|4|`JUNIOR_PROFESSIONAL`|×0.7|Junior, Trainee, Intern, Apprentice, Assistant, Associate|
|5|`OTHER`|×0.8|_(no keyword matched)_|



### **<mark>3. Monthly Income</mark>** 

<mark>Derived from cluster assignment and job category (values in BRL):</mark> 

||**`EXECUTIVE`**|**`SENIOR_PROFESSIONAL`**|**`MID_PROFESSIONAL`**|**`JUNIOR_PROFESSIONAL`**|**`OTHER`**|
|---|---|---|---|---|---|
|**CLUSTER_A**|30,000|20,000|12,000|8,000|10,000|
|**CLUSTER_B**|20,000|15,000|8,000|5,000|6,500|
|**CLUSTER_C**|10,000|7,000|5,000|3,000|4,000|
|**CLUSTER_D**|0|0|0|0|0|



### **<mark>4. Credit Limit Formula</mark>** 

```
approved_limit = round_to_nearest_100(
    min( base_limit × job_multiplier × penalty_factor, cluster_cap )
)
```

<mark>CLUSTER_D always yields</mark> `approved_limit = 0` <mark>.</mark> 

### **<mark>5. Penalty Rules</mark>** 

<mark>Applied after the job multiplier, before the cluster cap.</mark> 

|**Priority**|**Rule ID**|**Trigger**|**Effect**|
|---|---|---|---|
|1|`DEFAULT_DEBT_PENALTY`|`credit_default`or<br>`loan_default`in<br>`market_debt_types`|×0.5|



## **API** 

### **<mark>`POST /customers/classify`</mark>** 

Accept a customer object in the request body, classify it, and return the enriched record. The application is stateless — nothing is persisted between calls. 

**<mark>Request body:</mark>** <mark>Customer object (schema defined above).</mark> 

**<mark>Response:</mark>** <mark>The same object enriched with all calculated fields from the output contract.</mark> 

## **Testing (Required)** 

<mark>Both unit and integration tests are required.</mark> 

**<mark>Unit tests</mark>** <mark>must cover the core classification logic in isolation:</mark> 

Cluster assignment for each cluster, including boundary conditions (e.g. score exactly at threshold) Job category matching, including case-insensitivity and priority ordering 

Credit limit calculation: base formula, penalty application, cap enforcement, and `round_to_nearest_100` Monthly income lookup for all cluster × job category combinations CLUSTER_D denial (approved = false, approved_limit = 0) 

**<mark>Integration tests</mark>** <mark>must exercise the full request/response cycle:</mark> 

> `POST /customers/classify` with valid input returns correct output contract 

> `POST /customers/classify` with invalid or missing fields returns appropriate error responses All 6 sample customers from `expected-output.json` produce the exact expected output

## Local implementation evidence

The following evidence applies to the current repository implementation. The
official six-customer fixture and `expected-output.json` were not provided, so
their exact-output comparison remains unavailable and is not claimed here.

| Requirement | Status | Evidence |
|---|---|---|
| REQ-001 | VERIFIED | `src/Btg.CreditEngine.Api/Program.cs`; API integration tests; stateless request handling |
| REQ-002 | VERIFIED | `src/Btg.CreditEngine.Domain/Customer.cs`, `src/Btg.CreditEngine.Api/CustomerRequest.cs` |
| REQ-003 | VERIFIED | `src/Btg.CreditEngine.Domain/ClusterRules.cs`; `ClusterClassificationTests` |
| REQ-004 | VERIFIED | `src/Btg.CreditEngine.Domain/JobRules.cs`; `JobClassificationTests` |
| REQ-005 | VERIFIED | `src/Btg.CreditEngine.Domain/FinancialRules.cs`; income matrix tests |
| REQ-006 | VERIFIED | `CreditEngine` calculation and rounding tests; API result tests |
| REQ-007 | VERIFIED | penalty rule and penalty calculation tests |
| REQ-008 | VERIFIED | `Validation.cs`; API invalid-body and invalid-field tests; `CustomerValidationTests` proves debt flag and debt-type collection are validated independently |
| REQ-009 | VERIFIED | domain test projects covering boundaries, priority, formula, cap, rounding and denial |
| REQ-010 | VERIFIED | `CustomerEndpointTests` full request/response tests |
| REQ-011 | MISSING | Official six customers and `expected-output.json` are unavailable in this repository |
| REQ-012 | VERIFIED | `dotnet test Btg.CreditEngine.sln --configuration Release` |
| REQ-013 | VERIFIED | README documents prerequisites, restore/build, API startup, Bash and PowerShell endpoint usage, response shape, and one-command tests; `ai-journey/` identifies Codex and records two supported material interactions with decisions, corrections, lessons, and concrete repository evidence |
| REQ-014 | PARTIAL | [Persisted independent Reviewer PASS](reviews/independent-review.md) for commit `98cdf05`; the final Challenge Audit remains pending |

<mark>Your test suite must be runnable with a single command (e.g.</mark> `pytest` <mark>,</mark> `dotnet test` <mark>,</mark> `npm test` <mark>).</mark> 

## **Evaluation** 

|**Area**|**What we look for**|
|---|---|
|Problem Solving|Correct rule implementation, edge-case handling|
|Architecture|Separation of concerns, data-driven design|
|Code Quality|Readability, error handling, input validation|
|Testing|Coverage of classification paths and edge cases|



|**Area**|**What we look for**|
|---|---|
|Documentation|API docs, architecture decisions|
|AI Collaboration|How you used AI, critical thinking about AI output|



## **AI Journey (Required)** 

AI usage is expected and encouraged — we want to see how you think with it, not whether you avoided it. Include an `ai-journey/` folder in your repository documenting your process: 

```
ai-journey/
├── README.md      # Brief summary: which tools you used and for what
├── prompts.md     # Key prompts, what they produced, and how you iterated
└── learnings.md   # What worked, what didn't, what you'd do differently
```

> **`prompts.md`** <mark>— focus on the interesting interactions, not a full log:</mark> 

#### **`## Prompt: Cluster assignment logic`** 

```
**Tool:** GitHub Copilot / ChatGPT-4 / etc.
```

```
**What I asked:**
"Given a priority-ordered list of cluster conditions, write a function that
evaluates each cluster in order and returns the first match."
```

```
**What happened:**
The initial suggestion used nested if/else which became hard to maintain.
I asked for a data-driven approach instead and got a cleaner result.
```

```
**Final solution:**
```

```
[brief description or code snippet]
```

**`learnings.md`** <mark>— honest reflection on the experience:</mark> 

#### **`## What worked well`** 

- `AI was fast for generating test data and boilerplate` 

- `Helped explore regex patterns for job-title keyword matching` 

#### **`## What didn't work`** 

- `Rounding logic was subtly wrong on the first attempt — had to verify manually` 

- `AI missed the priority-order constraint in cluster evaluation` 

#### **`## What I'd do differently`** 

- `Prompt for edge cases explicitly before accepting a solution` 

- `Use AI earlier in the design phase, not just for implementation` 

<mark>The depth and honesty of this documentation matters as much as the code itself.</mark> 
