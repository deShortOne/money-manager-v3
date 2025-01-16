
namespace MoneyTracker.Contracts.Requests.Bill;
public class NewBillRequest(int payeeId, decimal amount, DateOnly nextDueDate, string frequency,
    int categoryId, int payerId)
{
    public int PayeeId { get; } = payeeId;
    public decimal Amount { get; } = amount;
    public DateOnly NextDueDate { get; } = nextDueDate;
    public string Frequency { get; } = frequency;
    public int CategoryId { get; } = categoryId;
    public int PayerId { get; } = payerId;
}
