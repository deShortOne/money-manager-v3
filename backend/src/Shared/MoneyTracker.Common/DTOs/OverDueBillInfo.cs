
namespace MoneyTracker.Common.DTOs;
public class OverDueBillInfo(int daysOverDue, DateOnly[] pastOccurences)
{
    public int DaysOverDue { get; } = daysOverDue;
    public DateOnly[] PastOccurences { get; } = pastOccurences;

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
        return HashCode.Combine(DaysOverDue, PastOccurences);
    }
}
