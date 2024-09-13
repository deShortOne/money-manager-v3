

namespace MoneyTracker.Shared.Models.Bill;
public class OverDueBillInfo(int daysOverDue, DateOnly[] pastOccurences)
{
    public int DaysOverDue { get; private set; } = daysOverDue;
    public DateOnly[] PastOccurences { get; private set; } = pastOccurences;

    public override bool Equals(object? obj)
    {
        var other = obj as OverDueBillInfo;

        if (other == null)
        {
            return false;
        }

        return DaysOverDue == other.DaysOverDue && PastOccurences.SequenceEqual(other.PastOccurences);
    }

    public override int GetHashCode()
    {
        return DaysOverDue;
    }
}
