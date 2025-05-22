using MoneyTracker.Queries.Application.Wage;
using MoneyTracker.Queries.Application.Wage.TaxCode;

namespace MoneyTracker.Queries.Tests.WageTests.Service.TaxCode.GivenARequestToTheTaxCodeSelector;
public sealed class WhenTheTaxCodeIsBR
{
    private readonly WageInterface _subject;

    public WhenTheTaxCodeIsBR()
    {
        _subject = TaxCodeSelector.SelectTaxCodeImplementorFrom("BR");
    }

    [Fact]
    public void ThenTheObjectReturnedIsOfTheCorrectType()
    {
        Assert.Equal(typeof(CalculateTaxCodeBR), _subject.GetType());
    }
}
