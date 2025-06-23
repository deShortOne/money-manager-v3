
namespace MoneyTracker.Commands.Domain.Entities.Bill;
public class BillEntity(int id, int payeeId, decimal amount, DateOnly nextDueDate, int monthDay,
    string frequency, int categoryId, int payerId)
{
    public int Id { get; } = id;
    public int PayeeId { get; } = payeeId;
    public decimal Amount { get; } = amount;
    public DateOnly NextDueDate { get; } = nextDueDate;
    public int MonthDay { get; } = monthDay;
    public string Frequency { get; } = frequency;
    public int CategoryId { get; } = categoryId;
    public int PayerId { get; } = payerId;

    public override bool Equals(object? obj)
    {
        var other = obj as BillEntity;

        if (other == null)
        {
            return false;
        }

        return Id == other.Id && PayeeId == other.PayeeId && Amount == other.Amount &&
            NextDueDate == other.NextDueDate && Frequency == other.Frequency &&
            CategoryId == other.CategoryId && MonthDay == other.MonthDay &&
            PayerId == other.PayerId;
    }

    public override int GetHashCode()
    {
        return Id;
    }
}
