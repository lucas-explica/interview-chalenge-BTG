namespace Btg.CreditEngine.Domain;

public sealed record JobCategoryRule(
    string Id,
    decimal Multiplier,
    IReadOnlyList<string> Keywords);

public sealed record JobClassification(string Id, decimal Multiplier);

public static class JobRules
{
    // Ordered highest-priority first. Matching is case-insensitive substring matching.
    public static IReadOnlyList<JobCategoryRule> Categories { get; } =
    [
        new("EXECUTIVE", 2.0m, ["CEO", "CFO", "CTO", "COO", "CIO", "CMO", "Chief", "President", "Vice President", "VP", "Director"]),
        new("SENIOR_PROFESSIONAL", 1.5m, ["Senior", "Lead", "Manager", "Coordinator", "Supervisor", "Principal"]),
        new("MID_PROFESSIONAL", 1.0m, ["Engineer", "Analyst", "Developer", "Specialist", "Designer", "Accountant", "Consultant", "Architect"]),
        new("JUNIOR_PROFESSIONAL", 0.7m, ["Junior", "Trainee", "Intern", "Apprentice", "Assistant", "Associate"]),
        new("OTHER", 0.8m, [])
    ];
}

public static class JobClassifier
{
    public static JobClassification Classify(string? jobTitle)
    {
        var category = JobRules.Categories
            .Where(category => category.Id != "OTHER")
            .FirstOrDefault(category => category.Keywords.Any(keyword =>
                jobTitle?.Contains(keyword, StringComparison.OrdinalIgnoreCase) == true));

        category ??= JobRules.Categories.Single(category => category.Id == "OTHER");
        return new JobClassification(category.Id, category.Multiplier);
    }
}
