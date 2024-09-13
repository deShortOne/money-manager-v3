

namespace MoneyTracker.Shared.Models.Bill;
public class OverDueBillInfo(int daysOverDue, int numberOfBillsOverDue, DateOnly[] pastOccurences)
{
    public int DaysOverDue { get; private set; } = daysOverDue;
    public int NumberOfBillsOverDue { get; private set; } = numberOfBillsOverDue;
    public DateOnly[] PastOccurences { get; private set; } = pastOccurences;

    public override bool Equals(object? obj)
    {
        var other = obj as OverDueBillInfo;

        if (other == null)
        {
            return false;
        }

        return DaysOverDue == other.DaysOverDue && NumberOfBillsOverDue == other.NumberOfBillsOverDue &&
            PastOccurences.SequenceEqual(other.PastOccurences);
    }

    public override int GetHashCode()
    {
        return DaysOverDue;
    }
}
