using Btg.CreditEngine.Domain;

namespace Btg.CreditEngine.Domain.Tests;

public sealed class ClusterClassificationTests
{
    [Fact]
    public void DataContainsUniquePriorityOrderedClustersAndCatchAll()
    {
        var rules = CreditRules.Clusters;

        Assert.Equal(["CLUSTER_A", "CLUSTER_B", "CLUSTER_C", "CLUSTER_D"], rules.Select(rule => rule.Id));
        Assert.Equal(rules.Count, rules.Select(rule => rule.Id).Distinct().Count());
        Assert.True(rules[^1].IsCatchAll);
        Assert.All(rules, rule => Assert.True(rule.BaseLimit >= 0 && rule.Cap >= rule.BaseLimit));
    }

    [Fact]
    public void DiamondRequiresHighScoreAgeRangeAndNoMarketDebt()
    {
        Assert.Equal("CLUSTER_A", ClusterClassifier.Classify(Customer(score: 700, age: 25)).Id);
        Assert.Equal("CLUSTER_A", ClusterClassifier.Classify(Customer(score: 800, age: 60)).Id);
        Assert.NotEqual("CLUSTER_A", ClusterClassifier.Classify(Customer(score: 700, age: 24)).Id);
        Assert.NotEqual("CLUSTER_A", ClusterClassifier.Classify(Customer(score: 700, age: 61)).Id);
        Assert.NotEqual("CLUSTER_A", ClusterClassifier.Classify(Customer(score: 700, hasMarketDebt: true, debtTypes: [MarketDebtTypes.Mortgage])).Id);
    }

    [Fact]
    public void GoldUsesInclusiveScoreAndAgeBoundariesAndExcludesDefaults()
    {
        Assert.Equal("CLUSTER_B", ClusterClassifier.Classify(Customer(score: 500, age: 18)).Id);
        Assert.Equal("CLUSTER_B", ClusterClassifier.Classify(Customer(score: 699, age: 65)).Id);
        Assert.Equal("CLUSTER_C", ClusterClassifier.Classify(Customer(score: 500, age: 17)).Id);
        Assert.Equal("CLUSTER_C", ClusterClassifier.Classify(Customer(score: 500, age: 66)).Id);
        Assert.Equal("CLUSTER_C", ClusterClassifier.Classify(Customer(score: 500, hasMarketDebt: true, debtTypes: [MarketDebtTypes.CreditDefault])).Id);
    }

    [Fact]
    public void SilverStartsAtInclusiveScoreBoundary()
    {
        Assert.Equal("CLUSTER_C", ClusterClassifier.Classify(Customer(score: 300, age: 10)).Id);
        Assert.Equal("CLUSTER_D", ClusterClassifier.Classify(Customer(score: 299, age: 10)).Id);
    }

    [Fact]
    public void HigherPriorityClusterWinsWhenConditionsOverlap()
    {
        var assignment = ClusterClassifier.Classify(Customer(score: 900, age: 40));

        Assert.Equal("CLUSTER_A", assignment.Id);
        Assert.Equal("Diamond", assignment.Name);
        Assert.Equal(50_000m, assignment.BaseLimit);
        Assert.Equal(100_000m, assignment.Cap);
    }

    [Fact]
    public void CatchAllClassifiesLowScoreCustomersAsBronze()
    {
        var assignment = ClusterClassifier.Classify(Customer(score: 0, age: 0));

        Assert.Equal("CLUSTER_D", assignment.Id);
        Assert.Equal("Bronze", assignment.Name);
        Assert.Equal(0m, assignment.BaseLimit);
        Assert.Equal(0m, assignment.Cap);
    }

    private static CustomerInput Customer(
        int score,
        int age = 30,
        bool hasMarketDebt = false,
        IReadOnlyList<string>? debtTypes = null) => new()
    {
        Age = age,
        Score = score,
        HasMarketDebt = hasMarketDebt,
        MarketDebtTypes = debtTypes ?? []
    };
}
