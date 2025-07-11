
using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using MoneyTracker.Commands.Domain.Entities.MessageQueuePolling;
using MoneyTracker.Commands.Domain.Repositories;
using MoneyTracker.Common.Result;

namespace MoneyTracker.Commands.Infrastructure.AWS;
public class SQSRepository : IMessageQueueRepository
{
    private readonly IAmazonSQS _amazonSQSClient;
    private readonly string _queueUrl;
    private readonly int _maxMessages;

    public SQSRepository(IAmazonSQS amazonSQSClient, string queueUrl, int maxMessages)
    {
        _amazonSQSClient = amazonSQSClient;
        _queueUrl = queueUrl;
        _maxMessages = maxMessages;
    }

    public async Task<ResultT<SuccessfulFileNamesAndFailedMessageIds>> GetFileNamesThatHaveBeenProcessed(CancellationToken cancellationToken)
    {
        var messageResponse = await _amazonSQSClient.ReceiveMessageAsync(
            new ReceiveMessageRequest()
            {
                QueueUrl = _queueUrl,
                MaxNumberOfMessages = _maxMessages,
                WaitTimeSeconds = 10,
            }, cancellationToken);

        var sqsMessages = messageResponse.Messages;
        if (sqsMessages is null)
            return Error.Failure("", "Failed to get messages from AWS");

        var successfulMessages = new List<SuccessfulMessageInfo>();
        var failedMessageIds = new List<Result>();
        foreach (var message in sqsMessages)
        {
            if (message.Body is null)
            {
                failedMessageIds.Add(Result.Failure(Error.Failure(message.MessageId, "Message body is invalid")));
                continue;
            }
            var body = JsonSerializer.Deserialize<MessageBody?>(message.Body);
            if (body is null || body.Records.Count == 0)
            {
                failedMessageIds.Add(Result.Failure(Error.Failure(message.MessageId, "Message body doesn't contain any records")));
            }
            else
            {
                successfulMessages.AddRange(body.Records.ConvertAll(x => new SuccessfulMessageInfo
                {
                    MessageId = message.MessageId,
                    Filename = x.S3.S3Object.Key,
                    QueueMessageId = message.ReceiptHandle
                }));
            }
        }

        return new SuccessfulFileNamesAndFailedMessageIds
        {
            SuccessfulFiles = successfulMessages,
            FailedMessageIds = failedMessageIds,
        };
    }

    public async Task DeleteMessage(string receiptHandle, CancellationToken cancellationToken)
    {
        await _amazonSQSClient.DeleteMessageAsync(new DeleteMessageRequest
        {
            QueueUrl = _queueUrl,
            ReceiptHandle = receiptHandle,
        }, cancellationToken);
    }
}
