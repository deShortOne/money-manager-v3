
namespace MoneyTracker.Commands.Domain.Entities.Transaction;
public class TransactionEntity(int id, int payeeId, decimal amount, DateOnly datePaid, int categoryId, int payerId)
{
    public int Id { get; } = id;
    public int PayeeId { get; } = payeeId;
    public decimal Amount { get; } = amount;
    public DateOnly DatePaid { get; } = datePaid;
    public int CategoryId { get; } = categoryId;
    public int PayerId { get; } = payerId;

    public override bool Equals(object? obj)
    {
        var other = obj as TransactionEntity;

        if (other == null)
        {
            return false;
        }

        return Id == other.Id &&
            PayeeId == other.PayeeId &&
            Amount == other.Amount &&
            DatePaid == other.DatePaid &&
            CategoryId == other.CategoryId &&
            PayerId == other.PayerId;
    }

    public override int GetHashCode()
    {
        return Id;
    }
}
