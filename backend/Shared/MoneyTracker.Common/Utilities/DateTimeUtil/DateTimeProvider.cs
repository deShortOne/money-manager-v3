
namespace MoneyTracker.Common.Utilities.DateTimeUtil;
public class DateTimeProvider : IDateTimeProvider
{
    public DateTime Now => DateTime.UtcNow;
}
