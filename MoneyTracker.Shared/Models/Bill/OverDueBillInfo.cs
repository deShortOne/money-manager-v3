
namespace MoneyTracker.Shared.Models.Bill;
public class OverDueBillInfo(int daysOverDue, int numberOfBillsOverDue)
{
    public int DaysOverDue { get; private set; } = daysOverDue;
    public int NumberOfBillsOverDue { get; private set; } = numberOfBillsOverDue;

    public override bool Equals(System.Object obj)
    {
        var other = obj as OverDueBillInfo;

        if (other == null)
        {
            return false;
        }

        return DaysOverDue == other.DaysOverDue && NumberOfBillsOverDue == other.NumberOfBillsOverDue;
    }
}
