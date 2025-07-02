
using Amazon.SQS;
using Amazon.SQS.Model;
using MoneyTracker.Commands.Infrastructure.AWS;
using Moq;

namespace MoneyTracker.Commands.Tests.RegisterTests.Repository.GivenARequestToGetMessagesFromTheQueue;
public class WhenEverythingIsValid : IAsyncLifetime
{
    private Mock<IAmazonSQS> _amazonSQSClient = new Mock<IAmazonSQS>();
    private string _queueUrl = "da URL will go here";
    private int _maxMessages = 5;
    private string _firstMessageId = "C7C844DB-EEC1-46D8-A0BA-2D78BE0781B3";
    private ReceiveMessageResponse _receiveMessageResponse = new ReceiveMessageResponse();

    private ReceiveMessageRequest _resultReceiveMessageRequest;
    private List<Message> _result;

    public async Task InitializeAsync()
    {
        _receiveMessageResponse.Messages = new List<Message>
        {
            new Message
            {
                MessageId = _firstMessageId,
            }
        };

        _amazonSQSClient.Setup(x => x.ReceiveMessageAsync(It.IsAny<ReceiveMessageRequest>(), It.IsAny<CancellationToken>()))
            .Callback((ReceiveMessageRequest rmr, CancellationToken _) => _resultReceiveMessageRequest = rmr)
            .ReturnsAsync(_receiveMessageResponse);

        var sut = new SQSRepository(_amazonSQSClient.Object, _queueUrl, _maxMessages);

        _result = await sut.ReceiveMessage(CancellationToken.None);
    }
    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    [Fact]
    public void ThenTheResponseIsCorrect()
    {
        Assert.Single(_receiveMessageResponse.Messages);
        Assert.Equal(_firstMessageId, _receiveMessageResponse.Messages[0].MessageId);
    }

    [Fact]
    public void ThenTheRequestPassedIsCorrect()
    {
        Assert.Equal(_queueUrl, _resultReceiveMessageRequest.QueueUrl);
        Assert.Equal(_maxMessages, _resultReceiveMessageRequest.MaxNumberOfMessages);
        Assert.Equal(10, _resultReceiveMessageRequest.WaitTimeSeconds);
    }
}
