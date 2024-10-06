
namespace MoneyTracker.Shared.Models.ServiceToRepository.Bill;
public class NewBillDTO(string payee, decimal amount, DateOnly nextDueDate, string frequency,
    int category, int monthDay, int accountId)
{
    public string Payee { get; private set; } = payee;
    public decimal Amount { get; private set; } = amount;
    public DateOnly NextDueDate { get; private set; } = nextDueDate;
    public string Frequency { get; private set; } = frequency;
    public int Category { get; private set; } = category;
    public int MonthDay { get; private set; } = monthDay;
    public int AccountId { get; } = accountId;

    public override bool Equals(object? obj)
    {
        var other = obj as NewBillDTO;
        if (other == null) return false;
        return Payee == other.Payee &&
            Amount == other.Amount &&
            NextDueDate == other.NextDueDate &&
            Frequency == other.Frequency &&
            Category == other.Category &&
            MonthDay == other.MonthDay &&
            AccountId == other.AccountId;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Payee, Payee, Amount, NextDueDate, Frequency, Category, MonthDay, AccountId);
    }
}
