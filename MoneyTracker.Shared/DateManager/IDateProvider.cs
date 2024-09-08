
namespace MoneyTracker.Shared.DateManager;
public interface IDateProvider
{
    DateOnly Now { get; }
}
