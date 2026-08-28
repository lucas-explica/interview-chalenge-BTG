using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Btg.CreditEngine.Api.Tests;

public sealed class ApiBootstrapTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient client;

    public ApiBootstrapTests(WebApplicationFactory<Program> factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task HealthEndpointStartsTheApiHost()
    {
        using var response = await client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("{\"status\":\"ok\"}", await response.Content.ReadAsStringAsync());
    }
}
