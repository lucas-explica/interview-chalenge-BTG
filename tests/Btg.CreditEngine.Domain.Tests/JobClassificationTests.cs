using Btg.CreditEngine.Domain;

namespace Btg.CreditEngine.Domain.Tests;

public sealed class JobClassificationTests
{
    [Fact]
    public void DataContainsAllCategoriesInPriorityOrder()
    {
        Assert.Equal(
            ["EXECUTIVE", "SENIOR_PROFESSIONAL", "MID_PROFESSIONAL", "JUNIOR_PROFESSIONAL", "OTHER"],
            JobRules.Categories.Select(category => category.Id));
        Assert.Equal(JobRules.Categories.Count, JobRules.Categories.Select(category => category.Id).Distinct().Count());
        Assert.Equal(0.8m, JobRules.Categories[^1].Multiplier);
    }

    [Theory]
    [InlineData("CEO", "EXECUTIVE", 2.0)]
    [InlineData("finance manager", "SENIOR_PROFESSIONAL", 1.5)]
    [InlineData("software ENGINEER", "MID_PROFESSIONAL", 1.0)]
    [InlineData("legal trainee", "JUNIOR_PROFESSIONAL", 0.7)]
    [InlineData("Entrepreneur", "OTHER", 0.8)]
    public void MatchesEachCategoryAndFallsBackToOther(string title, string expectedId, double expectedMultiplier)
    {
        var result = JobClassifier.Classify(title);

        Assert.Equal(expectedId, result.Id);
        Assert.Equal((decimal)expectedMultiplier, result.Multiplier);
    }

    [Fact]
    public void MatchingWorksCaseInsensitivelyAnywhereInTheTitle()
    {
        var result = JobClassifier.Classify("Regional principal data architect");

        Assert.Equal("SENIOR_PROFESSIONAL", result.Id);
    }

    [Fact]
    public void ExecutiveWinsOverSeniorWhenBothKeywordsArePresent()
    {
        var result = JobClassifier.Classify("Senior Vice President of Engineering");

        Assert.Equal("EXECUTIVE", result.Id);
        Assert.Equal(2.0m, result.Multiplier);
    }

    [Fact]
    public void NullOrBlankTitleUsesOtherCategory()
    {
        Assert.Equal("OTHER", JobClassifier.Classify(null).Id);
        Assert.Equal("OTHER", JobClassifier.Classify(" ").Id);
    }
}
