using MoneyTracker.PlatformService.Domain;
using MoneyTracker.PlatformService.DTOs;

namespace MoneyTracker.PlatformService;
public class EmptyMessageBusClient : IMessageBusClient
{
    public async ValueTask DisposeAsync()
    {
        await Task.CompletedTask;

        GC.SuppressFinalize(this);
    }

    public async Task PublishEvent(EventUpdate eventToUpdate, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }
}
