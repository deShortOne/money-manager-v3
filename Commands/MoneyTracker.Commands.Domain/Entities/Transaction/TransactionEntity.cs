
namespace MoneyTracker.Commands.Domain.Entities.Transaction;
public class TransactionEntity(int id, string payee, decimal amount, DateOnly datePaid, int categoryId, int accountId)
{
    public int Id { get; } = id;
    public string Payee { get; } = payee;
    public decimal Amount { get; } = amount;
    public DateOnly DatePaid { get; } = datePaid;
    public int CategoryId { get; } = categoryId;
    public int AccountId { get; } = accountId;

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
            CategoryId == other.CategoryId &&
            AccountId == other.AccountId;
    }

    public override int GetHashCode()
    {
        return Id;
    }
}
