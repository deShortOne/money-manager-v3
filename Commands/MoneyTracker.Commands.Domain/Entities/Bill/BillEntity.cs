
namespace MoneyTracker.Commands.Domain.Entities.Bill;
public class BillEntity(int id, string payee, decimal amount, DateOnly nextDueDate, int monthDay,
    string frequency, int categoryId, int accountId)
{
    public int Id { get; } = id;
    public string Payee { get; } = payee;
    public decimal Amount { get; } = amount;
    public DateOnly NextDueDate { get; } = nextDueDate;
    public int MonthDay { get; } = monthDay;
    public string Frequency { get; } = frequency;
    public int CategoryId { get; } = categoryId;
    public int AccountId { get; } = accountId;

    public override bool Equals(object? obj)
    {
        var other = obj as BillEntity;

        if (other == null)
        {
            return false;
        }

        return Id == other.Id && Payee == other.Payee && Amount == other.Amount &&
            NextDueDate == other.NextDueDate && Frequency == other.Frequency &&
            CategoryId == other.CategoryId && MonthDay == other.MonthDay &&
            AccountId == other.AccountId;
    }

    public override int GetHashCode()
    {
        return Id;
    }
}
