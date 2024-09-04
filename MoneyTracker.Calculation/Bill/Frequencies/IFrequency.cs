
using MoneyTracker.Shared.Models.Bill;

namespace MoneyTracker.Calculation.Bill.Frequencies;
internal interface IFrequency
{
    public bool MatchCommand(string frequency);
    public OverDueBillInfo? Calculate(DateOnly nextDueDate);
}
