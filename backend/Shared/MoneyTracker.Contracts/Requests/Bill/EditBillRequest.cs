
namespace MoneyTracker.Contracts.Requests.Bill;
public class EditBillRequest(int id, int? payeeId = null, decimal? amount = null, DateOnly? nextDueDate = null, string? frequency = null, int? categoryId = null, int? payerId = null)
{
    public int Id { get; } = id;
    public int? PayeeId { get; } = payeeId;
    public decimal? Amount { get; } = amount;
    public DateOnly? NextDueDate { get; } = nextDueDate;
    public string? Frequency { get; } = frequency;
    public int? CategoryId { get; } = categoryId;
    public int? PayerId { get; } = payerId;
}
