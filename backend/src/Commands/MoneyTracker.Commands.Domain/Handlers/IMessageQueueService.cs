
namespace MoneyTracker.Commands.Domain.Handlers;
public interface IMessageQueueService
{
    Task PollAsync(CancellationToken cancellationToken);
}
