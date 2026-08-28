namespace Btg.CreditEngine.Domain;

public sealed record ClusterRule(
    string Id,
    string Name,
    int? MinimumScore,
    int? MinimumAge,
    int? MaximumAge,
    bool RequiresNoMarketDebt,
    bool ExcludesDefaultDebt,
    decimal BaseLimit,
    decimal Cap,
    bool IsCatchAll = false)
{
    public bool Matches(CustomerInput customer)
    {
        if (IsCatchAll)
        {
            return true;
        }

        if (MinimumScore is not null && customer.Score < MinimumScore)
        {
            return false;
        }

        if (MinimumAge is not null && customer.Age < MinimumAge)
        {
            return false;
        }

        if (MaximumAge is not null && customer.Age > MaximumAge)
        {
            return false;
        }

        if (RequiresNoMarketDebt && customer.HasMarketDebt != false)
        {
            return false;
        }

        return !ExcludesDefaultDebt ||
            !(customer.MarketDebtTypes ?? []).Any(MarketDebtTypes.Default.Contains);
    }
}

public sealed record ClusterAssignment(
    string Id,
    string Name,
    decimal BaseLimit,
    decimal Cap);

public static class CreditRules
{
    // Ordered highest-priority first. Values are the challenge's rule table.
    public static IReadOnlyList<ClusterRule> Clusters { get; } =
    [
        new("CLUSTER_A", "Diamond", 700, 25, 60, true, false, 50_000m, 100_000m),
        new("CLUSTER_B", "Gold", 500, 18, 65, false, true, 20_000m, 40_000m),
        new("CLUSTER_C", "Silver", 300, null, null, false, false, 5_000m, 10_000m),
        new("CLUSTER_D", "Bronze", null, null, null, false, false, 0m, 0m, true)
    ];
}

public static class ClusterClassifier
{
    public static ClusterAssignment Classify(CustomerInput customer)
    {
        ArgumentNullException.ThrowIfNull(customer);

        var rule = CreditRules.Clusters.First(rule => rule.Matches(customer));
        return new ClusterAssignment(rule.Id, rule.Name, rule.BaseLimit, rule.Cap);
    }
}
