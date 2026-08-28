using System.Text.Json;
using Btg.CreditEngine.Api;
using Btg.CreditEngine.Domain;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.MapPost("/customers/classify", async (HttpRequest httpRequest) =>
{
    CustomerRequest? request;
    try
    {
        request = await JsonSerializer.DeserializeAsync<CustomerRequest>(httpRequest.Body);
    }
    catch (JsonException)
    {
        return Results.BadRequest(new ErrorResponse(
            "validation_error",
            [new("body", "Request body must be valid JSON.")]));
    }

    var validation = CustomerValidator.Validate(request?.ToDomain());
    if (!validation.IsValid)
    {
        return Results.BadRequest(new ErrorResponse(
            "validation_error",
            validation.Errors.Select(error => new ValidationErrorResponse(error.Field, error.Message)).ToArray()));
    }

    return Results.Ok(request);
});

app.Run();

public partial class Program;
