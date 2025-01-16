
namespace MoneyTracker.Contracts.Requests.Transaction;
public class NewTransactionRequest(int payeeId, decimal amount, DateOnly datePaid, int categoryId, int payerId)
{
    public int PayeeId { get; } = payeeId;
    public decimal Amount { get; } = amount;
    public DateOnly DatePaid { get; } = datePaid;
    public int CategoryId { get; } = categoryId;
    public int PayerId { get; } = payerId;
}
