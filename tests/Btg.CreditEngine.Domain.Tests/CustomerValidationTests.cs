using Btg.CreditEngine.Domain;

namespace Btg.CreditEngine.Domain.Tests;

public sealed class CustomerValidationTests
{
    [Fact]
    public void CompleteCustomerIsValid()
    {
        var result = CustomerValidator.Validate(ValidCustomer());

        Assert.True(result.IsValid);
    }

    [Fact]
    public void MissingRequiredFieldsAreReported()
    {
        var result = CustomerValidator.Validate(new CustomerInput());

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.Field == "id");
        Assert.Contains(result.Errors, error => error.Field == "location");
        Assert.Contains(result.Errors, error => error.Field == "market_debt_types");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(1001)]
    public void ScoresOutsideTheDocumentedRangeAreRejected(int score)
    {
        var result = CustomerValidator.Validate(ValidCustomer() with { Score = score });

        Assert.Contains(result.Errors, error => error.Field == "score");
    }

    [Fact]
    public void UnsupportedRegionAndDebtTypeAreRejected()
    {
        var result = CustomerValidator.Validate(ValidCustomer() with
        {
            MarketDebtTypes = ["unsupported_debt"],
            HasMarketDebt = true,
            Location = new LocationInput { City = "São Paulo", State = "SP", Region = "Leste" }
        });

        Assert.Contains(result.Errors, error => error.Field == "market_debt_types[0]");
        Assert.Contains(result.Errors, error => error.Field == "location.region");
    }

    [Fact]
    public void DebtFlagAndCollectionMustBeCoherent()
    {
        var result = CustomerValidator.Validate(ValidCustomer() with
        {
            HasMarketDebt = false,
            MarketDebtTypes = [MarketDebtTypes.Mortgage]
        });

        Assert.Contains(result.Errors, error => error.Field == "has_market_debt");
    }

    private static CustomerInput ValidCustomer() => new()
    {
        Id = "customer-1",
        Name = "Ana Silva",
        Age = 30,
        Score = 700,
        HasMarketDebt = false,
        MarketDebtTypes = [],
        Location = new LocationInput { City = "São Paulo", State = "SP", Region = "Sudeste" },
        JobTitle = "Engineer"
    };
}
