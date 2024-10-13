
namespace MoneyTracker.Shared.Models.ControllerToService.Bill;
public class NewBillRequestDTO(string payee, decimal amount, DateOnly nextDueDate, string frequency,
    int category, int accountId)
{
    public string Payee { get; } = payee;
    public decimal Amount { get; } = amount;
    public DateOnly NextDueDate { get; } = nextDueDate;
    public string Frequency { get; } = frequency;
    public int Category { get; } = category;
    public int AccountId { get; } = accountId;
}
