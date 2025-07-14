using MoneyTracker.PlatformService.DTOs;

namespace MoneyTracker.PlatformService.Domain;
public class EmptyEventProcessor : IEventProcessor
{
    public void ProcessEvent(EventUpdate eventUpdate, CancellationToken cancellationToken)
    {
        Console.WriteLine(eventUpdate);
    }
}
