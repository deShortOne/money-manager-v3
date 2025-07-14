
namespace MoneyTracker.Contracts.Requests.Transaction;
public class NewTransactionFromReceiptRequest(string fileid, int payeeId, decimal amount, DateOnly datePaid, int categoryId, int payerId)
{
    public string Fileid { get; } = fileid;
    public int PayeeId { get; } = payeeId;
    public decimal Amount { get; } = amount;
    public DateOnly DatePaid { get; } = datePaid;
    public int CategoryId { get; } = categoryId;
    public int PayerId { get; } = payerId;
}
