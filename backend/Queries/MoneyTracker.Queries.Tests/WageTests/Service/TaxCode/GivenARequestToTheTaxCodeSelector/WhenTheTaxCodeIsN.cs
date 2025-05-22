using MoneyTracker.Queries.Application.Wage;
using MoneyTracker.Queries.Application.Wage.TaxCode;

namespace MoneyTracker.Queries.Tests.WageTests.Service.TaxCode.GivenARequestToTheTaxCodeSelector;
public sealed class WhenTheTaxCodeIsN
{
    private readonly WageInterface _subject;

    public WhenTheTaxCodeIsN()
    {
        _subject = TaxCodeSelector.SelectTaxCodeImplementorFrom("1N");
    }

    [Fact]
    public void ThenTheObjectReturnedIsOfTheCorrectType()
    {
        Assert.Equal(typeof(CalculateProgressiveTaxAfterPersonalAllowance), _subject.GetType());
    }
}
