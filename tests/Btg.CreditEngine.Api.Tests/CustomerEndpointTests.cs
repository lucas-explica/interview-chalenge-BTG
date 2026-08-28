using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Btg.CreditEngine.Api.Tests;

public sealed class CustomerEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient client;

    public CustomerEndpointTests(WebApplicationFactory<Program> factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task ValidCustomerIsDeserializedWithTheCompleteSnakeCaseContract()
    {
        using var response = await client.PostAsJsonAsync("/customers/classify", ValidCustomer());
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var root = document.RootElement;

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("customer-1", root.GetProperty("id").GetString());
        Assert.Equal("Ana Silva", root.GetProperty("name").GetString());
        Assert.Equal(30, root.GetProperty("age").GetInt32());
        Assert.Equal(700, root.GetProperty("score").GetInt32());
        Assert.False(root.GetProperty("has_market_debt").GetBoolean());
        Assert.Empty(root.GetProperty("market_debt_types").EnumerateArray());
        Assert.Equal("São Paulo", root.GetProperty("location").GetProperty("city").GetString());
        Assert.Equal("SP", root.GetProperty("location").GetProperty("state").GetString());
        Assert.Equal("Sudeste", root.GetProperty("location").GetProperty("region").GetString());
        Assert.Equal("Engineer", root.GetProperty("job_title").GetString());
        Assert.Equal("CLUSTER_A", root.GetProperty("cluster_id").GetString());
        Assert.Equal("Diamond", root.GetProperty("cluster_name").GetString());
        Assert.Equal("MID_PROFESSIONAL", root.GetProperty("job_category").GetString());
        Assert.Equal(12_000m, root.GetProperty("monthly_income").GetDecimal());
        Assert.True(root.GetProperty("approved").GetBoolean());
        Assert.Equal(50_000m, root.GetProperty("approved_limit").GetDecimal());
    }

    [Fact]
    public async Task PenalizedAndDeniedRequestsReturnTheirOwnDeterministicResults()
    {
        using var penalizedResponse = await client.PostAsJsonAsync("/customers/classify", Customer(
            score: 500,
            age: 30,
            hasMarketDebt: true,
            debtTypes: ["credit_default"],
            jobTitle: "Senior Manager"));
        using var deniedResponse = await client.PostAsJsonAsync("/customers/classify", Customer(
            score: 100,
            age: 30,
            jobTitle: "CEO"));
        using var penalized = JsonDocument.Parse(await penalizedResponse.Content.ReadAsStringAsync());
        using var denied = JsonDocument.Parse(await deniedResponse.Content.ReadAsStringAsync());

        Assert.Equal(HttpStatusCode.OK, penalizedResponse.StatusCode);
        Assert.Equal("CLUSTER_C", penalized.RootElement.GetProperty("cluster_id").GetString());
        Assert.Equal(3_800m, penalized.RootElement.GetProperty("approved_limit").GetDecimal());
        Assert.True(penalized.RootElement.GetProperty("approved").GetBoolean());

        Assert.Equal(HttpStatusCode.OK, deniedResponse.StatusCode);
        Assert.Equal("CLUSTER_D", denied.RootElement.GetProperty("cluster_id").GetString());
        Assert.False(denied.RootElement.GetProperty("approved").GetBoolean());
        Assert.Equal(0m, denied.RootElement.GetProperty("approved_limit").GetDecimal());
    }

    [Fact]
    public async Task MissingBodyReturnsStableValidationError()
    {
        using var response = await client.PostAsync("/customers/classify", content: null);
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("validation_error", document.RootElement.GetProperty("error").GetString());
        Assert.Equal("body", document.RootElement.GetProperty("errors")[0].GetProperty("field").GetString());
    }

    [Fact]
    public async Task InvalidJsonReturnsStableValidationError()
    {
        using var content = new StringContent("{ invalid json");
        using var response = await client.PostAsync("/customers/classify", content);
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("validation_error", document.RootElement.GetProperty("error").GetString());
    }

    [Fact]
    public async Task InvalidCustomerReturnsFieldErrorsBeforeEvaluation()
    {
        using var response = await client.PostAsJsonAsync("/customers/classify", new { score = 1001 });
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains(
            document.RootElement.GetProperty("errors").EnumerateArray(),
            error => error.GetProperty("field").GetString() == "score");
    }

    private static object ValidCustomer() => new
    {
        id = "customer-1",
        name = "Ana Silva",
        age = 30,
        score = 700,
        has_market_debt = false,
        market_debt_types = Array.Empty<string>(),
        location = new { city = "São Paulo", state = "SP", region = "Sudeste" },
        job_title = "Engineer"
    };

    private static object Customer(
        int score,
        int age,
        bool hasMarketDebt = false,
        string[]? debtTypes = null,
        string jobTitle = "Engineer") => new
    {
        id = "customer-1",
        name = "Ana Silva",
        age,
        score,
        has_market_debt = hasMarketDebt,
        market_debt_types = debtTypes ?? [],
        location = new { city = "São Paulo", state = "SP", region = "Sudeste" },
        job_title = jobTitle
    };
}
