using Btg.CreditEngine.Domain;

namespace Btg.CreditEngine.Domain.Tests;

public sealed class FinancialCalculationTests
{
    [Fact]
    public void IncomeMatrixContainsEveryClusterAndCategoryCombination()
    {
        var categories = JobRules.Categories.Select(category => category.Id).ToArray();

        Assert.Equal(4, FinancialRules.MonthlyIncome.Count);
        Assert.All(FinancialRules.MonthlyIncome.Values, income =>
            Assert.Equal(categories, income.Keys));
        Assert.All(FinancialRules.MonthlyIncome["CLUSTER_D"].Values, income => Assert.Equal(0m, income));
    }

    [Fact]
    public void BaseLimitUsesJobMultiplierAndRoundsToHundred()
    {
        var result = CreditEngine.Evaluate(Customer("CLUSTER_A", "Engineer", 800, 30));

        Assert.Equal("MID_PROFESSIONAL", result.Job.Id);
        Assert.Equal(12_000m, result.MonthlyIncome);
        Assert.True(result.Approved);
        Assert.Equal(50_000m, result.ApprovedLimit);
    }

    [Fact]
    public void DefaultDebtPenaltyIsAppliedBeforeTheCap()
    {
        var result = CreditEngine.Evaluate(Customer("CLUSTER_C", "Senior Manager", 500, 30, true, [MarketDebtTypes.CreditDefault]));

        Assert.Equal("SENIOR_PROFESSIONAL", result.Job.Id);
        Assert.Equal("CLUSTER_C", result.Cluster.Id);
        Assert.Equal(3_800m, result.ApprovedLimit);
    }

    [Fact]
    public void CapIsAppliedBeforeRounding()
    {
        var result = CreditEngine.Evaluate(Customer("CLUSTER_B", "CEO", 500, 30, true, [MarketDebtTypes.Mortgage]));

        Assert.Equal(40_000m, result.ApprovedLimit);
    }

    [Theory]
    [InlineData(10_049, 10_000)]
    [InlineData(10_051, 10_100)]
    [InlineData(10_100, 10_100)]
    public void RoundsToNearestHundred(decimal amount, decimal expected)
    {
        Assert.Equal(expected, CreditEngine.RoundToNearestHundred(amount));
    }

    [Fact]
    public void BronzeIsAlwaysDeniedWithZeroLimit()
    {
        var result = CreditEngine.Evaluate(Customer("CLUSTER_D", "CEO", 100, 30));

        Assert.False(result.Approved);
        Assert.Equal(0m, result.ApprovedLimit);
        Assert.Equal(0m, result.MonthlyIncome);
    }

    private static CustomerInput Customer(
        string _,
        string jobTitle,
        int score,
        int age,
        bool hasMarketDebt = false,
        IReadOnlyList<string>? debtTypes = null) => new()
    {
        Age = age,
        Score = score,
        HasMarketDebt = hasMarketDebt,
        MarketDebtTypes = debtTypes ?? [],
        JobTitle = jobTitle
    };
}
