
namespace MoneyTracker.Commands.Domain.Entities.Bill;
public class EditBillEntity(int id, int? payee = null, decimal? amount = null, DateOnly? nextDueDate = null, int? monthDay = null, string? frequency = null, int? category = null, int? accountId = null)
{
    public int Id { get; } = id;
    public int? Payee { get; } = payee;
    public decimal? Amount { get; } = amount;
    public DateOnly? NextDueDate { get; } = nextDueDate;
    public int? MonthDay { get; } = monthDay;
    public string? Frequency { get; } = frequency;
    public int? CategoryId { get; } = category;
    public int? AccountId { get; } = accountId;

    public override bool Equals(object? obj)
    {
        var other = obj as EditBillEntity;
        if (other == null) return false;
        return Id == other.Id &&
            Payee == other.Payee &&
            Amount == other.Amount &&
            NextDueDate == other.NextDueDate &&
            MonthDay == other.MonthDay &&
            Frequency == other.Frequency &&
            CategoryId == other.CategoryId &&
            AccountId == other.AccountId;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Payee, Payee, Amount, NextDueDate, Frequency, CategoryId, AccountId);
    }
}
