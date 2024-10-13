
namespace MoneyTracker.Contracts.Requests.Bill;
public class NewBillRequest(string payee, decimal amount, DateOnly nextDueDate, string frequency,
    int categoryId, int accountId)
{
    public string Payee { get; } = payee;
    public decimal Amount { get; } = amount;
    public DateOnly NextDueDate { get; } = nextDueDate;
    public string Frequency { get; } = frequency;
    public int CategoryId { get; } = categoryId;
    public int AccountId { get; } = accountId;
}
