
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MoneyTracker.PlatformService.Domain;
using MoneyTracker.PlatformService.RabbitMQ;

namespace MoneyTracker.PlatformService;
public class PlatformServiceStartup
{
    public static void StartClient(WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration["Messaging:Lepus"]!;

        var messageBusClient = MessageBusClient.InitializeAsync(connectionString);
        messageBusClient.Wait();

        builder.Services
            .AddSingleton<IMessageBusClient>(_ => messageBusClient.Result);
    }

    public static void StartSubscriber(WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration["Messaging:Lepus"]!;

        builder.Services
            .AddSingleton<IEventProcessor, EventProcessor>()
            .AddHostedService(provider => MessageBusSubscriber.InitializeAsync(connectionString, provider.GetRequiredService<IEventProcessor>()).Result);
    }
}
