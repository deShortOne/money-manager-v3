using MoneyTracker.Commands.Application.BackgroundTask;
using MoneyTracker.Commands.Application.BackgroundTask.ResultingObject;
using MoneyTracker.Commands.Application.BackgroundTask.ResultingObject.Schemas;
using MoneyTracker.Commands.Application.BackgroundTask.ResultingObject.Schemas.V1;
using MoneyTracker.Commands.Application.BackgroundTask.ResultingObject.Schemas.V2;
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
    public Mock<IAccountCommandRepository> _mockAccountCommandRepository = new();
    public Mock<ICategoryCommandRepository> _mockCategoryCommandRepository = new();


    public MessageQueueService _messageQueueService;

    public MessageQueueServiceHelper()
    {
        var strategyHandlers = new List<IHandler> {
            new HandleObjectVersion1(_mockReceiptCommandRepository.Object),
            new HandleObjectVersion2(_mockReceiptCommandRepository.Object,
                _mockAccountCommandRepository.Object,
                _mockCategoryCommandRepository.Object),
        };

        _messageQueueService = new MessageQueueService(
            _mockMessageQueueRepository.Object,
            _mockReceiptCommandRepository.Object,
            _mockFileUploadRepository.Object,
            _mockPollingController.Object,
            new StrategyContext(strategyHandlers)
        );
    }

    public abstract Task InitializeAsync();
    public abstract Task DisposeAsync();
}
