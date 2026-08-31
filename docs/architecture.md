# Architecture

The API is a thin adapter around a stateless domain evaluation:

`HTTP/JSON API adapter -> Domain -> structured rule data`

The API maps the snake_case request and response contract, handles malformed
input, and invokes the domain once per request. The domain has no dependency
on HTTP, ASP.NET Core, or serialization, which keeps classification and
financial behavior deterministic and directly unit-testable. Nothing is
persisted between calls.

Cluster rules, job categories, income values, and penalty values are ordered
or keyed structured data in the domain. The evaluator interprets these tables
instead of embedding business values in nested `if/else` branches. Cluster
and job evaluation use first-match priority semantics. The current trade-off
is that this structured rule data is compiled with the application; it is
simple and sufficient for the challenge, but not runtime-editable.

Financial calculations use `decimal`. Credit limits are capped and rounded to
the nearest R$100 with deterministic `MidpointRounding.AwayFromZero` behavior.
CLUSTER_D is always denied with a zero limit.

There is intentionally no database, message broker, generic rules engine, or
runtime AI dependency: the API is stateless and the required calculation is
small, deterministic, and explainable. If future requirements justify it,
the rules could evolve to versioned external data with startup validation and
controlled rollout/audit, without changing the domain's evaluation boundary.
