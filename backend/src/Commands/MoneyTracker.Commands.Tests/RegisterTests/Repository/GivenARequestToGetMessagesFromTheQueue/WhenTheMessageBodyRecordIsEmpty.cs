
using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using MoneyTracker.Commands.Domain.Entities.MessageQueuePolling;
using MoneyTracker.Commands.Infrastructure.AWS;
using MoneyTracker.Common.Result;
using Moq;

namespace MoneyTracker.Commands.Tests.RegisterTests.Repository.GivenARequestToGetMessagesFromTheQueue;
public class WhenTheMessageBodyRecordIsEmpty : IAsyncLifetime
{
    private Mock<IAmazonSQS> _amazonSQSClient = new Mock<IAmazonSQS>();
    private string _queueUrl = "da URL will go here";
    private int _maxMessages = 5;
    private string _failedMessageMessageId = "C7C844DB-EEC1-46D8-A0BA-2D78BE0781B3";
    private ReceiveMessageResponse _receiveMessageResponse = new ReceiveMessageResponse();

    private ReceiveMessageRequest _resultReceiveMessageRequest;
    private ResultT<SuccessfulFileNamesAndFailedMessageIds> _result;

    public async Task InitializeAsync()
    {
        var messageBodyWithMissingRecord = new MessageBody
        {
            Records = new List<Infrastructure.AWS.Record>(),
        };
        var failedMessageDueToNullRecord = new Message
        {
            MessageId = _failedMessageMessageId,
            Body = JsonSerializer.Serialize(messageBodyWithMissingRecord),
        };

        _receiveMessageResponse.Messages = new List<Message>
        {
            failedMessageDueToNullRecord,
        };

        _amazonSQSClient.Setup(x => x.ReceiveMessageAsync(It.IsAny<ReceiveMessageRequest>(), It.IsAny<CancellationToken>()))
            .Callback((ReceiveMessageRequest rmr, CancellationToken _) => _resultReceiveMessageRequest = rmr)
            .ReturnsAsync(_receiveMessageResponse);

        var sut = new SQSRepository(_amazonSQSClient.Object, _queueUrl, _maxMessages);

        _result = await sut.GetFileNamesThatHaveBeenProcessed(CancellationToken.None);
    }
    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    [Fact]
    public void ThenThereAreNoErrors()
    {
        Assert.True(_result.IsSuccess);
    }

    [Fact]
    public void ThenTheMessageWithTheInvalidBodyIsHandledCorrectly()
    {
        var failedMessage = _result.Value.FailedMessageIds;
        Assert.Single(failedMessage);
        Assert.Equal(_failedMessageMessageId, failedMessage[0].Error.Code);
        Assert.Equal("Message body doesn't contain any records", failedMessage[0].Error.Description);
    }

    [Fact]
    public void ThenTheRequestPassedIsCorrect()
    {
        Assert.Equal(_queueUrl, _resultReceiveMessageRequest.QueueUrl);
        Assert.Equal(_maxMessages, _resultReceiveMessageRequest.MaxNumberOfMessages);
        Assert.Equal(10, _resultReceiveMessageRequest.WaitTimeSeconds);
    }
}
