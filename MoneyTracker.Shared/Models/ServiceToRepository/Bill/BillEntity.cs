
namespace MoneyTracker.Shared.Models.ServiceToRepository.Bill;
public class BillEntity(int id, string payee, decimal amount, DateOnly nextDueDate, string frequency,
    int category, int monthDay, int accountId)
{
    public int Id { get; } = id;
    public string Payee { get; private set; } = payee;
    public decimal Amount { get; private set; } = amount;
    public DateOnly NextDueDate { get; private set; } = nextDueDate;
    public string Frequency { get; private set; } = frequency;
    public int Category { get; private set; } = category;
    public int MonthDay { get; private set; } = monthDay;
    public int AccountId { get; } = accountId;

    public override bool Equals(object? obj)
    {
        var other = obj as BillEntity;
        if (other == null) return false;
        return Id == other.Id &&
            Payee == other.Payee &&
            Amount == other.Amount &&
            NextDueDate == other.NextDueDate &&
            Frequency == other.Frequency &&
            Category == other.Category &&
            MonthDay == other.MonthDay &&
            AccountId == other.AccountId;
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(Id);
        hash.Add(Payee);
        hash.Add(Amount);
        hash.Add(NextDueDate);
        hash.Add(Frequency);
        hash.Add(Category);
        hash.Add(MonthDay);
        hash.Add(AccountId);
        return hash.ToHashCode();
    }
}
