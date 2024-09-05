
using MoneyTracker.Shared.DateManager;
using MoneyTracker.Shared.Models.Bill;

namespace MoneyTracker.Calculation.Bill.Frequencies;
internal class Weekly : IFrequency
{
    public OverDueBillInfo? Calculate(DateOnly nextDueDate, IDateTimeProvider dateTimeProvider) => throw new NotImplementedException();
    public bool MatchCommand(string frequency) => frequency == "Weekly";
}
