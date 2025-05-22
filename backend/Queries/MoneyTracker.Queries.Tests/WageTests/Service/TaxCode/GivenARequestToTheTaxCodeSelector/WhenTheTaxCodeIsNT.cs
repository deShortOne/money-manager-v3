using MoneyTracker.Queries.Application.Wage;
using MoneyTracker.Queries.Application.Wage.TaxCode;

namespace MoneyTracker.Queries.Tests.WageTests.Service.TaxCode.GivenARequestToTheTaxCodeSelector;
public sealed class WhenTheTaxCodeIsNT
{
    private readonly WageInterface _subject;

    public WhenTheTaxCodeIsNT()
    {
        _subject = TaxCodeSelector.SelectTaxCodeImplementorFrom("NT");
    }

    [Fact]
    public void ThenTheObjectReturnedIsOfTheCorrectType()
    {
        Assert.Equal(typeof(CalculateTaxCodeNT), _subject.GetType());
    }
}
