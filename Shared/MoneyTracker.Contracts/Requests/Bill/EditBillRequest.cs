
namespace MoneyTracker.Contracts.Requests.Bill;
public class EditBillRequest(int id, string? payee = null, decimal? amount = null, DateOnly? nextDueDate = null, string? frequency = null, int? category = null, int? accountId = null)
{
    public int Id { get; } = id;
    public string? Payee { get; } = payee;
    public decimal? Amount { get; } = amount;
    public DateOnly? NextDueDate { get; } = nextDueDate;
    public string? Frequency { get; } = frequency;
    public int? Category { get; } = category;
    public int? AccountId { get; } = accountId;
}
