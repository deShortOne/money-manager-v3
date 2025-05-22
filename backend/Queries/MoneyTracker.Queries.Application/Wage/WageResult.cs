using MoneyTracker.Common.Utilities.MoneyUtil;

namespace MoneyTracker.Queries.Application.Wage;
public record WageResult
{
    public Money TaxableIncome { get; set; }
    public Money TotalTaxPayable { get; set; }
    public Money StudentLoanAmount { get; set; }
    public Money Pension { get; set; }
    public Money NationalInsurance { get; set; }
    public Money TotalDeduction
    {
        get
        {
            var items = new List<Money> {
                TotalTaxPayable,
                StudentLoanAmount,
                Pension,
                NationalInsurance,
            };

            var total = items
                .Where(x => x is not null)
                .Sum(x => x.Amount);

            return Money.From(total);
        }
    }
}
