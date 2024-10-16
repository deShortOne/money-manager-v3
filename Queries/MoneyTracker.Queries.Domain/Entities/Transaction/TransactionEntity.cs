
namespace MoneyTracker.Queries.Domain.Entities.Transaction;
public class TransactionEntity(int id, string payee, decimal amount, DateOnly datePaid, string categoryName, string accountName)
{
    public int Id { get; } = id;
    public string Payee { get; } = payee;
    public decimal Amount { get; } = amount;
    public DateOnly DatePaid { get; } = datePaid;
    public string CategoryName { get; } = categoryName;
    public string AccountName { get; } = accountName;

    public override bool Equals(object? obj)
    {
        var other = obj as TransactionEntity;

        if (other == null)
        {
            return false;
        }

        return Id == other.Id &&
            Payee == other.Payee &&
            Amount == other.Amount &&
            DatePaid == other.DatePaid &&
            CategoryName == other.CategoryName &&
            AccountName == other.AccountName;
    }

    public override int GetHashCode()
    {
        return Id;
    }
}
