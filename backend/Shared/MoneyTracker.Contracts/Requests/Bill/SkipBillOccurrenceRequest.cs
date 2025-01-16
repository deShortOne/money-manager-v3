
namespace MoneyTracker.Contracts.Requests.Bill;
public class SkipBillOccurrenceRequest(int id, DateOnly skipDatePastThisDate)
{
    public int Id { get; } = id;
    public DateOnly SkipDatePastThisDate { get; } = skipDatePastThisDate;
}
