using MoneyTracker.PlatformService.DTOs;

namespace MoneyTracker.PlatformService.Domain;
public interface IMessageBusClient : IAsyncDisposable
{
    Task PublishEvent(EventUpdate eventToUpdate, CancellationToken cancellationToken);
}
