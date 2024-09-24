
namespace MoneyTracker.Shared.Models.ControllerToService.Bill;
public class SkipBillOccurrenceRequestDTO(int id, DateOnly skipDatePastThisDate)
{
    public int Id { get; private set; } = id;
    public DateOnly SkipDatePastThisDate { get; private set; } = skipDatePastThisDate;
}
