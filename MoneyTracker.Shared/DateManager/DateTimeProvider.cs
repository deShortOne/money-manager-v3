
namespace MoneyTracker.Shared.DateManager;
public class DateTimeProvider : IDateTimeProvider
{
    public DateTime Now => DateTime.UtcNow;
}
