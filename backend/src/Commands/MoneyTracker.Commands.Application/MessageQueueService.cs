
using MoneyTracker.Commands.Domain.Handlers;
using MoneyTracker.Commands.Domain.Repositories;

namespace MoneyTracker.Commands.Application;
public class MessageQueueService : IMessageQueueService
{
    private readonly IMessageQueueRepository _messageQueueRepository;

    public MessageQueueService(IMessageQueueRepository messageQueueRepository)
    {
        _messageQueueRepository = messageQueueRepository;
    }
    public async Task PollAsync(CancellationToken cancellationToken)
    {
        var messages = await _messageQueueRepository.ReceiveMessage(cancellationToken);
        if (messages == null)
        {
            Console.WriteLine("There are x messages");
            return;
        }

        Console.WriteLine($"There are {messages.Count} messages");
        foreach (var message in messages)
        {
            Console.WriteLine(message.MessageId);
            Console.WriteLine(message.Body);
            Console.WriteLine("------------");
        }
        Console.WriteLine("============");
    }
}
