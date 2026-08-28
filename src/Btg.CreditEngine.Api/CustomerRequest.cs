using System.Text.Json.Serialization;
using Btg.CreditEngine.Domain;

namespace Btg.CreditEngine.Api;

public sealed record CustomerRequest
{
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("age")]
    public int? Age { get; init; }

    [JsonPropertyName("score")]
    public int? Score { get; init; }

    [JsonPropertyName("has_market_debt")]
    public bool? HasMarketDebt { get; init; }

    [JsonPropertyName("market_debt_types")]
    public string[]? MarketDebtTypes { get; init; }

    [JsonPropertyName("location")]
    public LocationRequest? Location { get; init; }

    [JsonPropertyName("job_title")]
    public string? JobTitle { get; init; }

    public CustomerInput ToDomain() => new()
    {
        Id = Id,
        Name = Name,
        Age = Age,
        Score = Score,
        HasMarketDebt = HasMarketDebt,
        MarketDebtTypes = MarketDebtTypes,
        Location = Location?.ToDomain(),
        JobTitle = JobTitle
    };
}

public sealed record LocationRequest
{
    [JsonPropertyName("city")]
    public string? City { get; init; }

    [JsonPropertyName("state")]
    public string? State { get; init; }

    [JsonPropertyName("region")]
    public string? Region { get; init; }

    public LocationInput ToDomain() => new() { City = City, State = State, Region = Region };
}

public sealed record ErrorResponse(
    [property: JsonPropertyName("error")] string Error,
    [property: JsonPropertyName("errors")] IReadOnlyList<ValidationErrorResponse> Errors);

public sealed record ValidationErrorResponse(
    [property: JsonPropertyName("field")] string Field,
    [property: JsonPropertyName("message")] string Message);

public sealed record CustomerResponse
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("age")]
    public required int Age { get; init; }

    [JsonPropertyName("score")]
    public required int Score { get; init; }

    [JsonPropertyName("has_market_debt")]
    public required bool HasMarketDebt { get; init; }

    [JsonPropertyName("market_debt_types")]
    public required IReadOnlyList<string> MarketDebtTypes { get; init; }

    [JsonPropertyName("location")]
    public required LocationRequest Location { get; init; }

    [JsonPropertyName("job_title")]
    public required string JobTitle { get; init; }

    [JsonPropertyName("cluster_id")]
    public required string ClusterId { get; init; }

    [JsonPropertyName("cluster_name")]
    public required string ClusterName { get; init; }

    [JsonPropertyName("job_category")]
    public required string JobCategory { get; init; }

    [JsonPropertyName("monthly_income")]
    public required decimal MonthlyIncome { get; init; }

    [JsonPropertyName("approved")]
    public required bool Approved { get; init; }

    [JsonPropertyName("approved_limit")]
    public required decimal ApprovedLimit { get; init; }

    public static CustomerResponse From(CustomerRequest request, CreditEvaluation evaluation) => new()
    {
        Id = request.Id!,
        Name = request.Name!,
        Age = request.Age!.Value,
        Score = request.Score!.Value,
        HasMarketDebt = request.HasMarketDebt!.Value,
        MarketDebtTypes = request.MarketDebtTypes!,
        Location = request.Location!,
        JobTitle = request.JobTitle!,
        ClusterId = evaluation.Cluster.Id,
        ClusterName = evaluation.Cluster.Name,
        JobCategory = evaluation.Job.Id,
        MonthlyIncome = evaluation.MonthlyIncome,
        Approved = evaluation.Approved,
        ApprovedLimit = evaluation.ApprovedLimit
    };
}
