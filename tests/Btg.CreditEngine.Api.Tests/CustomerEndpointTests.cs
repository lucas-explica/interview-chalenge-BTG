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
}
