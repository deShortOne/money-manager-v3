
namespace MoneyTracker.Shared.Models.RepositoryToService.Bill;
public class BillEntityDTO(int id, string payee, decimal amount, DateOnly nextDueDate,
    string frequency, string category, int monthDay, string accountName)
{
    public int Id { get; private set; } = id;
    public string Payee { get; private set; } = payee;
    public decimal Amount { get; private set; } = amount;
    public DateOnly NextDueDate { get; private set; } = nextDueDate;
    public string Frequency { get; private set; } = frequency;
    public string Category { get; private set; } = category;
    public int MonthDay { get; } = monthDay;
    public string AccountName { get; } = accountName;

    public override bool Equals(object? obj)
    {
        var other = obj as BillEntityDTO;

        if (other == null)
        {
            return false;
        }

        return Id == other.Id && Payee == other.Payee && Amount == other.Amount &&
            NextDueDate == other.NextDueDate && Frequency == other.Frequency &&
            Category == other.Category && MonthDay == other.MonthDay &&
            AccountName == other.AccountName;
    }

    public override int GetHashCode()
    {
        return Id;
    }
}
