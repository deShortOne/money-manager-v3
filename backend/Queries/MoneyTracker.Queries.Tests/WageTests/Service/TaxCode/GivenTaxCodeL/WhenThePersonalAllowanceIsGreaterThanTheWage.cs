using MoneyTracker.Common.Utilities.MoneyUtil;
using MoneyTracker.Queries.Application.Wage;
using MoneyTracker.Queries.Application.Wage.TaxCode;

namespace MoneyTracker.Queries.Tests.WageTests.Service.TaxCode.GivenTaxCodeL;
public sealed class WhenThePersonalAllowanceIsGreaterThanTheWage
{
    private WageResult _subject;

    public WhenThePersonalAllowanceIsGreaterThanTheWage()
    {
        var personalAllowance = Money.From(13000);

        var subject = new TaxCodeL(personalAllowance);

        _subject = subject.CalculateYearlyWage(Money.From(12000));
    }

    [Fact]
    public void ThenTheTaxableIncomeIs0()
    {
        Assert.Equal(Money.Zero, _subject.TaxableIncome);
    }

    [Fact]
    public void ThenTheTotalTaxPayableIs0()
    {
        Assert.Equal(Money.Zero, _subject.TotalTaxPayable);
    }
}
