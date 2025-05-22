using MoneyTracker.Queries.Application.Wage;
using MoneyTracker.Queries.Application.Wage.TaxCode;

namespace MoneyTracker.Queries.Tests.WageTests.Service.TaxCode.GivenARequestToTheTaxCodeSelector;
public sealed class WhenTheTaxCodeIsL
{
    private readonly IWageCalculator _subject;

    public WhenTheTaxCodeIsL()
    {
        _subject = TaxCodeSelector.SelectTaxCodeImplementorFrom("1L");
    }

    [Fact]
    public void ThenTheObjectReturnedIsOfTheCorrectType()
    {
        Assert.Equal(typeof(CalculateProgressiveTaxAfterPersonalAllowance), _subject.GetType());
    }
}
