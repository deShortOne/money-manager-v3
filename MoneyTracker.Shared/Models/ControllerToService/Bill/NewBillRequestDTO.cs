
namespace MoneyTracker.Shared.Models.ControllerToService.Bill;
public class NewBillRequestDTO(string payee, decimal amount, DateOnly nextDueDate, string frequency,
    int category, int monthDay, int accountId)
{
    public string Payee { get; private set; } = payee;
    public decimal Amount { get; private set; } = amount;
    public DateOnly NextDueDate { get; private set; } = nextDueDate;
    public string Frequency { get; private set; } = frequency;
    public int Category { get; private set; } = category;
    public int MonthDay { get; private set; } = monthDay;
    public int AccountId { get; } = accountId;
}
