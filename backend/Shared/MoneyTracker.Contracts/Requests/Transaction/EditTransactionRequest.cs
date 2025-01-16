
namespace MoneyTracker.Contracts.Requests.Transaction;
public class EditTransactionRequest(int id, int? payeeId = null, decimal? amount = null,
        DateOnly? datePaid = null, int? categoryId = null, int? payerId = null)
{

    public int Id { get; } = id;
    public int? PayeeId { get; } = payeeId;
    public decimal? Amount { get; } = amount;
    public DateOnly? DatePaid { get; } = datePaid;
    public int? CategoryId { get; } = categoryId;
    public int? PayerId { get; } = payerId;
}
