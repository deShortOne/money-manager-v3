using MoneyTracker.Queries.Application.Wage;
using MoneyTracker.Queries.Application.Wage.TaxCode;

namespace MoneyTracker.Queries.Tests.WageTests.Service.TaxCode.GivenARequestToTheTaxCodeSelector;
public sealed class WhenTheTaxCodeIsD1
{
    private readonly IWageCalculator _subject;

    public WhenTheTaxCodeIsD1()
    {
        _subject = TaxCodeSelector.SelectTaxCodeImplementorFrom("D1");
    }

    [Fact]
    public void ThenTheObjectReturnedIsOfTheCorrectType()
    {
        Assert.Equal(typeof(CalculateTaxCodeD1), _subject.GetType());
    }
}
