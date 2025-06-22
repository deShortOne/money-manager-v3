using System.Diagnostics.CodeAnalysis;

namespace MoneyTracker.Common.Utilities.DateTimeUtil;
[ExcludeFromCodeCoverage]
public class DateTimeProvider : IDateTimeProvider
{
    public DateTime Now => DateTime.UtcNow;
}
