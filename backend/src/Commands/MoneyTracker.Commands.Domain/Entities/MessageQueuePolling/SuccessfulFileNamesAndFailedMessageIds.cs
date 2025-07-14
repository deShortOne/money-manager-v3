
using MoneyTracker.Common.Result;

namespace MoneyTracker.Commands.Domain.Entities.MessageQueuePolling;
public class SuccessfulFileNamesAndFailedMessageIds
{
    public required List<SuccessfulMessageInfo> SuccessfulFiles { get; init; }
    public required List<Result> FailedMessageIds { get; set; }
}

public class SuccessfulMessageInfo
{
    public required string MessageId { get; init; }
    public required string Filename { get; set; }
    public required string QueueMessageId { get; init; }
}
