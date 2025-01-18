
using MoneyTracker.PlatformService.DTOs;

namespace MoneyTracker.PlatformService.Domain;
public interface IEventProcessor
{
    public void ProcessEvent(EventUpdate eventUpdate);
}
