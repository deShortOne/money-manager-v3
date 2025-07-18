
using Microsoft.Extensions.Hosting;
using MoneyTracker.Commands.Domain.Entities.MessageQueuePolling;
using MoneyTracker.Commands.Domain.Handlers;

namespace MoneyTracker.Commands.Application.BackgroundTask;
public class MessagePollingWorker : BackgroundService
{
    private readonly IMessageQueueService _messageQueueService;
    private readonly IPollingController _pollingController;

    public MessagePollingWorker(IMessageQueueService messageQueueService, IPollingController pollingController)
    {
        _messageQueueService = messageQueueService;
        _pollingController = pollingController;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        Console.WriteLine("Polling started");
        while (!ct.IsCancellationRequested)
        {
            if (_pollingController.ShouldPoll)
            {
                Console.WriteLine("Polling now");
                await _messageQueueService.PollAsync(ct);
            }
            else
            {
                Console.WriteLine("Not polling now 51");
            }
            await Task.Delay(TimeSpan.FromSeconds(10), ct);
        }
        Console.WriteLine("Polling stopped!!");
    }
}
