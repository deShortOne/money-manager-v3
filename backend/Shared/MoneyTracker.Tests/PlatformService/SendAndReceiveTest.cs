
using Microsoft.Extensions.Hosting;
using MoneyTracker.Authentication.DTOs;
using MoneyTracker.PlatformService.Domain;
using MoneyTracker.PlatformService.DTOs;
using MoneyTracker.PlatformService.RabbitMQ;
using Moq;
using Testcontainers.RabbitMq;

namespace MoneyTracker.Tests.PlatformService;
public sealed class SendAndReceiveTest : IAsyncLifetime
{
    private readonly RabbitMqContainer _rabbitMqContainer = new RabbitMqBuilder()
        .WithDockerEndpoint("tcp://localhost:2375")
        .Build();

    public async Task InitializeAsync()
    {
        await _rabbitMqContainer.StartAsync();

        await _rabbitMqContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _rabbitMqContainer.DisposeAsync().AsTask();
    }

    [Fact]
    public async Task ConfirmMessageBusCanSendData()
    {
        var client = new MessageBusClient(_rabbitMqContainer.GetConnectionString());

        var mockEventProcessor = new Mock<IEventProcessor>();
        IHostedService subscriber = new MessageBusSubscriber(_rabbitMqContainer.GetConnectionString(), mockEventProcessor.Object);
        await subscriber.StartAsync(CancellationToken.None);

        var eventToPublish = new EventUpdate(new AuthenticatedUser(1), Guid.NewGuid().ToString());
        await client.PublishEvent(eventToPublish, CancellationToken.None);

        await Task.Delay(5000); // f l a k e y y y y y

        mockEventProcessor.Verify(x => x.ProcessEvent(eventToPublish), Times.Once);
    }
}
