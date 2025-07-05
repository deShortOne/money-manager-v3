using MoneyTracker.Commands.Domain.Entities.MessageQueuePolling;
using MoneyTracker.Common.Result;

namespace MoneyTracker.Commands.Domain.Handlers;
public interface IMessageQueueService
{
    Task<ResultT<MessageQueueResult>> PollAsync(CancellationToken cancellationToken);
}
