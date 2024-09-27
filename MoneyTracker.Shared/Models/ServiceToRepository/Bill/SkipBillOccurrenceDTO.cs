
namespace MoneyTracker.Shared.Models.ServiceToRepository.Bill;
public class SkipBillOccurrenceDTO(int id, DateOnly skipDatePastThisDate)
{
    public int Id { get; private set; } = id;
    public DateOnly SkipDatePastThisDate { get; private set; } = skipDatePastThisDate;
}
