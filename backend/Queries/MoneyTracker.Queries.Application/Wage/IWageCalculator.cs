using MoneyTracker.Common.Utilities.MoneyUtil;

namespace MoneyTracker.Queries.Application.Wage;
public interface IWageCalculator
{
    public PreTaxGrossIncomeResult CalculatePreTaxGrossIncome(Money grossYearlyWage);
    public WageResult CalculateYearlyWage(Money grossYearlyWage);
}
