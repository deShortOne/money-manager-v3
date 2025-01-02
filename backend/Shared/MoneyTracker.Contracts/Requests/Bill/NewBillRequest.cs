
namespace MoneyTracker.Contracts.Requests.Bill;
public class NewBillRequest(int payee, decimal amount, DateOnly nextDueDate, string frequency,
    int categoryId, int payer)
{
    public int Payee { get; } = payee;
    public decimal Amount { get; } = amount;
    public DateOnly NextDueDate { get; } = nextDueDate;
    public string Frequency { get; } = frequency;
    public int CategoryId { get; } = categoryId;
    public int Payer { get; } = payer;
}
