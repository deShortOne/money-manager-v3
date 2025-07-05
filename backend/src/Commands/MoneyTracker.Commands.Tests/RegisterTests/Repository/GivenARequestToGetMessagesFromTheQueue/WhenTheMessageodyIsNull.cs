
using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using MoneyTracker.Commands.Application.AWS;
using MoneyTracker.Commands.Domain.Entities.MessageQueuePolling;
using MoneyTracker.Commands.Infrastructure.AWS;
using MoneyTracker.Common.Result;
using Moq;

namespace MoneyTracker.Commands.Tests.RegisterTests.Repository.GivenARequestToGetMessagesFromTheQueue;
public class WhenTheMessageodyIsNull : IAsyncLifetime
{
    private Mock<IAmazonSQS> _amazonSQSClient = new Mock<IAmazonSQS>();
    private string _queueUrl = "da URL will go here";
    private int _maxMessages = 5;
    private ReceiveMessageResponse _receiveMessageResponse = new ReceiveMessageResponse();

    private ReceiveMessageRequest _resultReceiveMessageRequest;
    private ResultT<SuccessfulFileNamesAndFailedMessageIds> _result;

    public async Task InitializeAsync()
    {
        _receiveMessageResponse.Messages = null;

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
    public void ThenThereAreErrorsAreReturned()
    {
        Assert.True(_result.HasError);
        Assert.Equal("Failed to get messages from AWS", _result.Error!.Description);
    }

    [Fact]
    public void ThenTheRequestPassedIsCorrect()
    {
        Assert.Equal(_queueUrl, _resultReceiveMessageRequest.QueueUrl);
        Assert.Equal(_maxMessages, _resultReceiveMessageRequest.MaxNumberOfMessages);
        Assert.Equal(10, _resultReceiveMessageRequest.WaitTimeSeconds);
    }
}
