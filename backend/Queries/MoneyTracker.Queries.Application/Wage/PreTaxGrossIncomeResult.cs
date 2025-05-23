using MoneyTracker.Common.Utilities.MoneyUtil;

namespace MoneyTracker.Queries.Application.Wage;
public sealed class PreTaxGrossIncomeResult
{
    public Money Pension { get; set; }
    public Money TotalDeduction
    {
        get
        {
            var items = new List<Money> {
                Pension,
            };

            var total = items
                .Where(x => x is not null)
                .Sum(x => x.Amount);

            return Money.From(total);
        }
    }
}
