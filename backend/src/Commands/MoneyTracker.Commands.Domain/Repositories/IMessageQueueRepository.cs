
using Amazon.SQS.Model;

namespace MoneyTracker.Commands.Domain.Repositories;
public interface IMessageQueueRepository
{
    Task<List<Message>> ReceiveMessage(CancellationToken ct);
    Task DeleteMessage(string receiptHandle, CancellationToken ct);
}
