
namespace MoneyTracker.Shared.Models.Bill;
public class NewBillDTO(string payee, decimal amount, DateOnly nextDueDate, string frequency, int category)
{
    public string Payee { get; private set; } = payee;
    public decimal Amount { get; private set; } = amount;
    public DateOnly NextDueDate { get; private set; } = nextDueDate;
    public string Frequency { get; private set; } = frequency;
    public int Category { get; private set; } = category;
}
