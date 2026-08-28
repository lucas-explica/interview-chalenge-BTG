namespace Btg.CreditEngine.Domain;

public sealed record CustomerInput
{
    public string? Id { get; init; }
    public string? Name { get; init; }
    public int? Age { get; init; }
    public int? Score { get; init; }
    public bool? HasMarketDebt { get; init; }
    public IReadOnlyList<string>? MarketDebtTypes { get; init; }
    public LocationInput? Location { get; init; }
    public string? JobTitle { get; init; }
}

public sealed record LocationInput
{
    public string? City { get; init; }
    public string? State { get; init; }
    public string? Region { get; init; }
}

public static class MarketDebtTypes
{
    public const string CreditCard = "credit_card";
    public const string PersonalLoan = "personal_loan";
    public const string Mortgage = "mortgage";
    public const string CreditDefault = "credit_default";
    public const string LoanDefault = "loan_default";

    public static readonly IReadOnlySet<string> All = new HashSet<string>(StringComparer.Ordinal)
    {
        CreditCard,
        PersonalLoan,
        Mortgage,
        CreditDefault,
        LoanDefault
    };

    public static readonly IReadOnlySet<string> Default = new HashSet<string>(StringComparer.Ordinal)
    {
        CreditDefault,
        LoanDefault
    };
}

public static class CustomerRegions
{
    public static readonly IReadOnlySet<string> All = new HashSet<string>(StringComparer.Ordinal)
    {
        "Norte",
        "Nordeste",
        "Centro-Oeste",
        "Sudeste",
        "Sul"
    };
}
