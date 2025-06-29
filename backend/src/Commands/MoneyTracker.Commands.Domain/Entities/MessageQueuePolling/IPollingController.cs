
namespace MoneyTracker.Commands.Domain.Entities.MessageQueuePolling;
public interface IPollingController
{
    bool ShouldPoll { get; }
    public void EnablePolling();
    public void DisablePolling();
}

public class PollingController : IPollingController
{
    public bool ShouldPoll { get; private set; } = false;
    public void EnablePolling() => ShouldPoll = true;
    public void DisablePolling() => ShouldPoll = false;
}
