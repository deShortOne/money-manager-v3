
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
#if RUN_LOCAL
        .WithDockerEndpoint("tcp://localhost:2375")
#endif
        .WithImage("rabbitmq:3.11")
        .WithCleanUp(true)
        .Build();

    public async Task InitializeAsync()
    {
        await _rabbitMqContainer.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await _rabbitMqContainer.DisposeAsync();
    }

    [Fact]
    public async Task ConfirmMessageBusCanSendData()
    {
        var client = await MessageBusClient.InitializeAsync(_rabbitMqContainer.GetConnectionString());

        var mockEventProcessor = new Mock<IEventProcessor>();
        IHostedService subscriber = await MessageBusSubscriber.InitializeAsync(_rabbitMqContainer.GetConnectionString(), mockEventProcessor.Object);
        await subscriber.StartAsync(CancellationToken.None);

        var eventToPublish = new EventUpdate(new AuthenticatedUser(1), Guid.NewGuid().ToString());
        await client.PublishEvent(eventToPublish, CancellationToken.None);

        await Task.Delay(5000); // f l a k e y y y y y

        mockEventProcessor.Verify(x => x.ProcessEvent(eventToPublish, It.IsAny<CancellationToken>()), Times.Once);
    }
}
