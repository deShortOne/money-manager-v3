
namespace MoneyTracker.Shared.Models.Bill;
public class BillDTO(int id, string payee, decimal amount, DateOnly nextDueDate, string frequency, string category)
{
    public int Id { get; private set; } = id;
    public string Payee { get; private set; } = payee;
    public decimal Amount { get; private set; } = amount;
    public DateOnly NextDueDate { get; private set; } = nextDueDate;
    public string Frequency { get; private set; } = frequency;
    public string Category { get; private set; } = category;
}
