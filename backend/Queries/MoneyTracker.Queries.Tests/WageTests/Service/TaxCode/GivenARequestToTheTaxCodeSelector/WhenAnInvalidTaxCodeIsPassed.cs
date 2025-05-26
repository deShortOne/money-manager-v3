using MoneyTracker.Queries.Application.Wage.TaxCode;

namespace MoneyTracker.Queries.Tests.WageTests.Service.TaxCode.GivenARequestToTheTaxCodeSelector;
public sealed class WhenAnInvalidTaxCodeIsPassed
{
    [Fact]
    public void ThenExceptionIsThrown()
    {
        var error = Assert.Throws<NotImplementedException>(() => TaxCodeSelector.SelectTaxCodeImplementorFrom("99Popo"));

        Assert.Equal("Invalid tax code: 99Popo", error.Message);
    }
}
