using MoneyTracker.Commands.Domain.Entities.MessageQueuePolling;
using MoneyTracker.Common.Result;
using Moq;

namespace MoneyTracker.Commands.Tests.RegisterTests.Repository.GivenARequestForMessageQueueServiceToPoll.V1;
public class WhenTheMessageQueueRepositoryFails : MessageQueueServiceHelper
{
    private readonly string _messageQueueRepositoryErrorDescription = "error message desc goes here";

    private ResultT<MessageQueueResult> _result;

    public override async Task InitializeAsync()
    {
        _mockMessageQueueRepository
            .Setup(x => x.GetFileNamesThatHaveBeenProcessed(CancellationToken.None))
            .ReturnsAsync(Error.Failure("", _messageQueueRepositoryErrorDescription));

        _result = await _messageQueueService.PollAsync(CancellationToken.None);
    }

    public override Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public void ThenAnErrorReturns()
    {
        Assert.True(_result.HasError);
        Assert.Equal(_messageQueueRepositoryErrorDescription, _result.Error!.Description);
    }
}
