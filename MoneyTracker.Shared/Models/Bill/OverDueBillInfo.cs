
namespace MoneyTracker.Shared.Models.Bill;
public class OverDueBillInfo(int daysOverDue, int numberOfBillsOverDue)
{
    public int daysOverDue { get; private set; } = daysOverDue;
    public int numberOfBillsOverDue { get; private set; } = numberOfBillsOverDue;

    public override bool Equals(System.Object obj)
    {
        var other = obj as OverDueBillInfo;

        if (other == null)
        {
            return false;
        }

        return numberOfBillsOverDue == other.numberOfBillsOverDue;
    }

    public override int GetHashCode()
    {
        return daysOverDue;
    }
}
