
namespace MoneyTracker.Shared.DateManager;
public class DateProvider : IDateProvider
{
    public DateOnly Now => DateOnly.FromDateTime(DateTime.Now);
}
