using MoneyTracker.Common.Utilities.MoneyUtil;
using MoneyTracker.Queries.Application.Wage;
using MoneyTracker.Queries.Application.Wage.TaxCode;

namespace MoneyTracker.Queries.Tests.WageTests.Service.TaxCode;
public sealed class WhenTaxCodeIsNT
{
    private WageResult _subject;

    public WhenTaxCodeIsNT()
    {
        var subject = new CalculateTaxCodeNT();

        _subject = subject.CalculateYearlyWage(Money.From(12000));
    }

    [Fact]
    public void ThenTheTaxableIncomeIs0()
    {
        Assert.Equal(Money.Zero, _subject.TaxableIncome);
    }

    [Fact]
    public void ThenTheTotalTaxPayable0()
    {
        Assert.Equal(Money.Zero, _subject.TotalTaxPayable);
    }
}
