
using MoneyTracker.Common.Utilities.CalculationUtil.Frequencies;

namespace MoneyTracker.Queries.Domain.Entities.Transaction;
public class TransactionEntity(int id,
    int payeeId,
    string payeeName,
    decimal amount,
    DateOnly datePaid,
    int categoryId,
    string categoryName,
    int payerId,
    string payerName)
{
    public int Id { get; } = id;
    public int PayeeId { get; } = payeeId;
    public string PayeeName { get; } = payeeName;
    public decimal Amount { get; } = amount;
    public DateOnly DatePaid { get; } = datePaid;
    public int CategoryId { get; } = categoryId;
    public string CategoryName { get; } = categoryName;
    public int PayerId { get; } = payerId;
    public string PayerName { get; } = payerName;

    public override bool Equals(object? obj)
    {
        var other = obj as TransactionEntity;

        if (other == null)
        {
            return false;
        }

        return Id == other.Id
            && PayeeId == other.PayeeId
            && PayeeName == other.PayeeName
            && Amount == other.Amount
            && DatePaid == other.DatePaid
            && CategoryId == other.CategoryId
            && CategoryName == other.CategoryName
            && PayerId == other.PayerId
            && PayerName == other.PayerName;
    }

    public override int GetHashCode()
    {
        HashCode hash = new();
        hash.Add(Id);
        hash.Add(PayeeId);
        hash.Add(PayeeName);
        hash.Add(Amount);
        hash.Add(DatePaid);
        hash.Add(CategoryId);
        hash.Add(CategoryName);
        hash.Add(PayerId);
        hash.Add(PayerName);
        return hash.ToHashCode();
    }
}
