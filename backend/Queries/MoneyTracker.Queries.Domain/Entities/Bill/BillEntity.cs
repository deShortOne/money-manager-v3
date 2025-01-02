
namespace MoneyTracker.Queries.Domain.Entities.Bill;
public class BillEntity(int id, string payee, decimal amount, DateOnly nextDueDate, int monthDay,
    string frequency, string categoryName, string payer)
{
    public int Id { get; } = id;
    public string Payee { get; } = payee;
    public decimal Amount { get; } = amount;
    public DateOnly NextDueDate { get; } = nextDueDate;
    public int MonthDay { get; } = monthDay;
    public string Frequency { get; } = frequency;
    public string CategoryName { get; } = categoryName;
    public string Payer { get; } = payer;

    public override bool Equals(object? obj)
    {
        var other = obj as BillEntity;

        if (other == null)
        {
            return false;
        }

        return Id == other.Id && Payee == other.Payee && Amount == other.Amount &&
            NextDueDate == other.NextDueDate && Frequency == other.Frequency &&
            CategoryName == other.CategoryName && MonthDay == other.MonthDay &&
            Payee == other.Payee;
    }

    public override int GetHashCode()
    {
        return Id;
    }
}
