
using MoneyTracker.Commands.Domain.Entities.MessageQueuePolling;
using MoneyTracker.Commands.Domain.Repositories;
using MoneyTracker.Common.Result;

namespace MoneyTracker.Commands.Application.Fake;
public class FakeSQS : IMessageQueueRepository
{
    public Task DeleteMessage(string receiptHandle, CancellationToken cancellationToken) => Task.CompletedTask;
    public async Task<ResultT<SuccessfulFileNamesAndFailedMessageIds>> GetFileNamesThatHaveBeenProcessed(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;

        return new SuccessfulFileNamesAndFailedMessageIds
        {
            FailedMessageIds = [],
            SuccessfulFiles = [
                new SuccessfulMessageInfo
                {
                    Filename = SomethingInTheMiddle.LastFileId,
                    MessageId = string.Empty,
                    QueueMessageId = string.Empty,
                }
            ],
        };
    }
}
