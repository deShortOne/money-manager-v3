
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

        builder.Services
            .AddSingleton<IMessageBusClient>(_ => new MessageBusClient(connectionString));
    }

    public static void StartSubscriber(WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration["Messaging:Lepus"]!;

        builder.Services
            .AddSingleton<IEventProcessor, EventProcessor>()
            .AddHostedService(provider => new MessageBusSubscriber(connectionString, provider.GetRequiredService<IEventProcessor>()));
    }
}
