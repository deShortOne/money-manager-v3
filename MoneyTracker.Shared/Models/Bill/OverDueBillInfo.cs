
namespace MoneyTracker.Shared.Models.Bill;
public class OverDueBillInfo(int daysOverDue, int numberOfBillsOverDue)
{
    public int daysOverDue { get; private set; } = daysOverDue;
    public int numberOfBillsOverDue { get; private set; } = numberOfBillsOverDue;
}
