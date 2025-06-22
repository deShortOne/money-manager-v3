
namespace MoneyTracker.Queries.Domain.Entities.Bill;
public class BillEntity(int id,
    int payeeId,
    string payeeName,
    decimal amount,
    DateOnly nextDueDate,
    int monthDay,
    string frequency,
    int categoryId,
    string categoryName,
    int payerId,
    string payerName
    )
{
    public int Id { get; } = id;
    public int PayeeId { get; } = payeeId;
    public string PayeeName { get; } = payeeName;
    public decimal Amount { get; } = amount;
    public DateOnly NextDueDate { get; } = nextDueDate;
    public int MonthDay { get; } = monthDay;
    public string Frequency { get; } = frequency;
    public int CategoryId { get; } = categoryId;
    public string CategoryName { get; } = categoryName;
    public int PayerId { get; } = payerId;
    public string PayerName { get; } = payerName;

    public override bool Equals(object? obj)
    {
        var other = obj as BillEntity;

        if (other == null)
        {
            return false;
        }

        return Id == other.Id
            && PayeeId == other.PayeeId
            && PayeeName == other.PayeeName
            && Amount == other.Amount
            && NextDueDate == other.NextDueDate
            && Frequency == other.Frequency
            && CategoryId == other.CategoryId
            && CategoryName == other.CategoryName
            && MonthDay == other.MonthDay
            && PayerId == other.PayerId
            && PayerName == other.PayerName;
    }

    public override int GetHashCode()
    {
        HashCode hash = new();
        hash.Add(Id);
        hash.Add(PayeeId);
        hash.Add(PayeeName);
        hash.Add(Amount);
        hash.Add(NextDueDate);
        hash.Add(Frequency);
        hash.Add(CategoryId);
        hash.Add(CategoryName);
        hash.Add(MonthDay);
        hash.Add(PayerId);
        hash.Add(PayerName);
        return hash.ToHashCode();
    }
}
