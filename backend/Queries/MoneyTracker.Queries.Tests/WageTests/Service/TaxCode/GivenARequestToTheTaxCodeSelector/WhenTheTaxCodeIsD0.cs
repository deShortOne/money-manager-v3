using MoneyTracker.Queries.Application.Wage;
using MoneyTracker.Queries.Application.Wage.TaxCode;

namespace MoneyTracker.Queries.Tests.WageTests.Service.TaxCode.GivenARequestToTheTaxCodeSelector;
public sealed class WhenTheTaxCodeIsD0
{
    private readonly WageInterface _subject;

    public WhenTheTaxCodeIsD0()
    {
        _subject = TaxCodeSelector.SelectTaxCodeImplementorFrom("D0");
    }

    [Fact]
    public void ThenTheObjectReturnedIsOfTheCorrectType()
    {
        Assert.Equal(typeof(CalculateTaxCodeD0), _subject.GetType());
    }
}
