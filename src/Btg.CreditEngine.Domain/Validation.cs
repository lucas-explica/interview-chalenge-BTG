namespace Btg.CreditEngine.Domain;

public sealed record ValidationError(string Field, string Message);

public sealed class ValidationResult
{
    private ValidationResult(IReadOnlyList<ValidationError> errors)
    {
        Errors = errors;
    }

    public IReadOnlyList<ValidationError> Errors { get; }
    public bool IsValid => Errors.Count == 0;

    public static ValidationResult Valid() => new([]);

    public static ValidationResult Invalid(IEnumerable<ValidationError> errors) =>
        new(errors.ToArray());
}

public static class CustomerValidator
{
    public static ValidationResult Validate(CustomerInput? customer)
    {
        if (customer is null)
        {
            return ValidationResult.Invalid([new("body", "Request body is required.")]);
        }

        var errors = new List<ValidationError>();
        RequireText(customer.Id, "id", errors);
        RequireText(customer.Name, "name", errors);
        RequireText(customer.JobTitle, "job_title", errors);

        if (customer.Age is null)
        {
            errors.Add(new("age", "The field is required."));
        }
        else if (customer.Age < 0)
        {
            errors.Add(new("age", "Age must be zero or greater."));
        }

        if (customer.Score is null)
        {
            errors.Add(new("score", "The field is required."));
        }
        else if (customer.Score is < 0 or > 1000)
        {
            errors.Add(new("score", "Score must be between 0 and 1000."));
        }

        if (customer.HasMarketDebt is null)
        {
            errors.Add(new("has_market_debt", "The field is required."));
        }

        ValidateDebtTypes(customer, errors);
        ValidateLocation(customer.Location, errors);

        return errors.Count == 0 ? ValidationResult.Valid() : ValidationResult.Invalid(errors);
    }

    private static void RequireText(string? value, string field, ICollection<ValidationError> errors)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            errors.Add(new(field, "The field is required."));
        }
    }

    private static void ValidateDebtTypes(CustomerInput customer, ICollection<ValidationError> errors)
    {
        if (customer.MarketDebtTypes is null)
        {
            errors.Add(new("market_debt_types", "The field is required."));
            return;
        }

        for (var index = 0; index < customer.MarketDebtTypes.Count; index++)
        {
            var debtType = customer.MarketDebtTypes[index];
            if (!MarketDebtTypes.All.Contains(debtType))
            {
                errors.Add(new($"market_debt_types[{index}]", "The debt type is not supported."));
            }
        }

        if (customer.HasMarketDebt == true && customer.MarketDebtTypes.Count == 0)
        {
            errors.Add(new("market_debt_types", "At least one debt type is required when has_market_debt is true."));
        }

        if (customer.HasMarketDebt == false && customer.MarketDebtTypes.Count > 0)
        {
            errors.Add(new("has_market_debt", "Must be true when market_debt_types is not empty."));
        }

        if (customer.MarketDebtTypes.Count != customer.MarketDebtTypes.Distinct(StringComparer.Ordinal).Count())
        {
            errors.Add(new("market_debt_types", "Debt types must not be duplicated."));
        }
    }

    private static void ValidateLocation(LocationInput? location, ICollection<ValidationError> errors)
    {
        if (location is null)
        {
            errors.Add(new("location", "The field is required."));
            return;
        }

        RequireText(location.City, "location.city", errors);
        RequireText(location.State, "location.state", errors);
        if (!string.IsNullOrWhiteSpace(location.State) &&
            (location.State.Length != 2 || location.State.Any(character => !char.IsLetter(character))))
        {
            errors.Add(new("location.state", "State must be a two-letter abbreviation."));
        }

        if (string.IsNullOrWhiteSpace(location.Region))
        {
            errors.Add(new("location.region", "The field is required."));
        }
        else if (!CustomerRegions.All.Contains(location.Region))
        {
            errors.Add(new("location.region", "The region is not supported."));
        }
    }
}
