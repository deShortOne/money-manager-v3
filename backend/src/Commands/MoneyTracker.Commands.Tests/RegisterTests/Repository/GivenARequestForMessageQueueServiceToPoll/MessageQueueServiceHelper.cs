
using MoneyTracker.Commands.Application;
using MoneyTracker.Commands.Domain.Entities.MessageQueuePolling;
using MoneyTracker.Commands.Domain.Repositories;
using Moq;

namespace MoneyTracker.Commands.Tests.RegisterTests.Repository.GivenARequestForMessageQueueServiceToPoll;
public abstract class MessageQueueServiceHelper : IAsyncLifetime
{
    public Mock<IMessageQueueRepository> _mockMessageQueueRepository = new();
    public Mock<IReceiptCommandRepository> _mockReceiptCommandRepository = new();
    public Mock<IFileUploadRepository> _mockFileUploadRepository = new();
    public Mock<IPollingController> _mockPollingController = new();

    public MessageQueueService _messageQueueService;

    public MessageQueueServiceHelper()
    {
        _messageQueueService = new MessageQueueService(
            _mockMessageQueueRepository.Object,
            _mockReceiptCommandRepository.Object,
            _mockFileUploadRepository.Object,
            _mockPollingController.Object
            );
    }

    public abstract Task InitializeAsync();
    public abstract Task DisposeAsync();
}
