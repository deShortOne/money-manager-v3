using MoneyTracker.Queries.Application.Wage;
using MoneyTracker.Queries.Application.Wage.TaxCode;

namespace MoneyTracker.Queries.Tests.WageTests.Service.TaxCode.GivenARequestToTheTaxCodeSelector;
public sealed class WhenTheTaxCodeIsM
{
    private readonly IWageCalculator _subject;

    public WhenTheTaxCodeIsM()
    {
        _subject = TaxCodeSelector.SelectTaxCodeImplementorFrom("1M");
    }

    [Fact]
    public void ThenTheObjectReturnedIsOfTheCorrectType()
    {
        Assert.Equal(typeof(CalculateProgressiveTaxAfterPersonalAllowance), _subject.GetType());
    }
}
