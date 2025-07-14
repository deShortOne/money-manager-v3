
using Amazon.SQS.Model;
using MoneyTracker.Commands.Domain.Entities.MessageQueuePolling;
using MoneyTracker.Common.Result;

namespace MoneyTracker.Commands.Domain.Repositories;
public interface IMessageQueueRepository
{
    Task<ResultT<SuccessfulFileNamesAndFailedMessageIds>> GetFileNamesThatHaveBeenProcessed(CancellationToken cancellationToken);
    Task DeleteMessage(string receiptHandle, CancellationToken cancellationToken);
}
