namespace Btg.CreditEngine.Domain;

public sealed record PenaltyRule(string Id, decimal Factor, IReadOnlySet<string> TriggerDebtTypes);

public sealed record CreditEvaluation(
    ClusterAssignment Cluster,
    JobClassification Job,
    decimal MonthlyIncome,
    bool Approved,
    decimal ApprovedLimit);

public static class FinancialRules
{
    public static PenaltyRule DefaultDebtPenalty { get; } =
        new("DEFAULT_DEBT_PENALTY", 0.5m, MarketDebtTypes.Default);

    public static IReadOnlyDictionary<string, IReadOnlyDictionary<string, decimal>> MonthlyIncome { get; } =
        new Dictionary<string, IReadOnlyDictionary<string, decimal>>(StringComparer.Ordinal)
        {
            ["CLUSTER_A"] = Income(("EXECUTIVE", 30_000m), ("SENIOR_PROFESSIONAL", 20_000m), ("MID_PROFESSIONAL", 12_000m), ("JUNIOR_PROFESSIONAL", 8_000m), ("OTHER", 10_000m)),
            ["CLUSTER_B"] = Income(("EXECUTIVE", 20_000m), ("SENIOR_PROFESSIONAL", 15_000m), ("MID_PROFESSIONAL", 8_000m), ("JUNIOR_PROFESSIONAL", 5_000m), ("OTHER", 6_500m)),
            ["CLUSTER_C"] = Income(("EXECUTIVE", 10_000m), ("SENIOR_PROFESSIONAL", 7_000m), ("MID_PROFESSIONAL", 5_000m), ("JUNIOR_PROFESSIONAL", 3_000m), ("OTHER", 4_000m)),
            ["CLUSTER_D"] = Income(("EXECUTIVE", 0m), ("SENIOR_PROFESSIONAL", 0m), ("MID_PROFESSIONAL", 0m), ("JUNIOR_PROFESSIONAL", 0m), ("OTHER", 0m))
        };

    private static IReadOnlyDictionary<string, decimal> Income(params (string Category, decimal Value)[] values) =>
        values.ToDictionary(value => value.Category, value => value.Value, StringComparer.Ordinal);
}

public static class CreditEngine
{
    public static CreditEvaluation Evaluate(CustomerInput customer)
    {
        ArgumentNullException.ThrowIfNull(customer);

        var cluster = ClusterClassifier.Classify(customer);
        var job = JobClassifier.Classify(customer.JobTitle);
        var monthlyIncome = FinancialRules.MonthlyIncome[cluster.Id][job.Id];
        var hasDefaultDebt = (customer.MarketDebtTypes ?? []).Any(FinancialRules.DefaultDebtPenalty.TriggerDebtTypes.Contains);

        if (cluster.Id == "CLUSTER_D")
        {
            return new(cluster, job, monthlyIncome, false, 0m);
        }

        var penalty = hasDefaultDebt ? FinancialRules.DefaultDebtPenalty.Factor : 1m;
        var uncappedLimit = cluster.BaseLimit * job.Multiplier * penalty;
        var cappedLimit = Math.Min(uncappedLimit, cluster.Cap);

        return new(cluster, job, monthlyIncome, true, RoundToNearestHundred(cappedLimit));
    }

    public static decimal RoundToNearestHundred(decimal amount) =>
        Math.Round(amount / 100m, 0, MidpointRounding.AwayFromZero) * 100m;
}
