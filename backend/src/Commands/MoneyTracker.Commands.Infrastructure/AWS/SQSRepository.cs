
using Amazon.SQS;
using Amazon.SQS.Model;
using MoneyTracker.Commands.Domain.Repositories;

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

    public async Task<List<Message>> ReceiveMessage(CancellationToken ct)
    {
        var messageResponse = await _amazonSQSClient.ReceiveMessageAsync(
            new ReceiveMessageRequest()
            {
                QueueUrl = _queueUrl,
                MaxNumberOfMessages = _maxMessages,
                WaitTimeSeconds = 10,
            }, ct);

        return messageResponse.Messages;
    }
}
