using MoneyTracker.Common.Result;

namespace MoneyTracker.Commands.Domain.Entities.MessageQueuePolling;
public class MessageQueueResult
{
    public required List<string> SuccessfullyProcessedFileIds {  get; set; }
    public required List<Result> FailedProcessedFileIds {  get; set; }
}
