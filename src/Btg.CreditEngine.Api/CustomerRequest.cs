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
