
namespace MoneyTracker.Commands.Domain.Entities.Bill;
public class EditBillEntity(int id,
    int? payeeId = null,
    decimal? amount = null,
    DateOnly? nextDueDate = null,
    int? monthDay = null,
    string? frequency = null,
    int? category = null,
    int? payerId = null)
{
    public int Id { get; } = id;
    public int? PayeeId { get; } = payeeId;
    public decimal? Amount { get; } = amount;
    public DateOnly? NextDueDate { get; } = nextDueDate;
    public int? MonthDay { get; } = monthDay;
    public string? Frequency { get; } = frequency;
    public int? CategoryId { get; } = category;
    public int? PayerId { get; } = payerId;

    public override bool Equals(object? obj)
    {
        var other = obj as EditBillEntity;
        if (other == null) return false;
        return Id == other.Id &&
            PayeeId == other.PayeeId &&
            Amount == other.Amount &&
            NextDueDate == other.NextDueDate &&
            MonthDay == other.MonthDay &&
            Frequency == other.Frequency &&
            CategoryId == other.CategoryId &&
            PayerId == other.PayerId;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, PayeeId, Amount, NextDueDate, MonthDay, Frequency, CategoryId, PayerId);
    }
}
