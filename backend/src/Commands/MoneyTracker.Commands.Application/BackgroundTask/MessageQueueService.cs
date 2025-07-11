
using System.Text.Json;
using System.Text.Json.Serialization;
using MoneyTracker.Commands.Application.BackgroundTask.ResultingObject;
using MoneyTracker.Commands.Application.BackgroundTask.ResultingObject.Schemas;
using MoneyTracker.Commands.Application.BackgroundTask.ResultingObject.Schemas.V1;
using MoneyTracker.Commands.Domain.Entities.MessageQueuePolling;
using MoneyTracker.Commands.Domain.Handlers;
using MoneyTracker.Commands.Domain.Repositories;
using MoneyTracker.Common.Result;
using MoneyTracker.Common.Values;

namespace MoneyTracker.Commands.Application.BackgroundTask;
public class MessageQueueService : IMessageQueueService
{
    private readonly IMessageQueueRepository _messageQueueRepository;
    private readonly IReceiptCommandRepository _receiptCommandRepository;
    private readonly IFileUploadRepository _fileUploadRepository;
    private readonly IPollingController _pollingController;

    public MessageQueueService(IMessageQueueRepository messageQueueRepository,
        IReceiptCommandRepository receiptCommandRepository,
        IFileUploadRepository fileUploadRepository,
        IPollingController pollingController)
    {
        _messageQueueRepository = messageQueueRepository;
        _receiptCommandRepository = receiptCommandRepository;
        _fileUploadRepository = fileUploadRepository;
        _pollingController = pollingController;
    }

    public async Task<ResultT<MessageQueueResult>> PollAsync(CancellationToken cancellationToken)
    {
        var successfulFileNamesAndFailedMessageIds = await _messageQueueRepository.GetFileNamesThatHaveBeenProcessed(cancellationToken);
        if (successfulFileNamesAndFailedMessageIds.HasError)
        {
            Console.WriteLine(successfulFileNamesAndFailedMessageIds.Error!.Description);
            return successfulFileNamesAndFailedMessageIds.Error;
        }

        var successfulMessageIds = new List<string>();
        var failedMessageIdsWithReason = successfulFileNamesAndFailedMessageIds.Value.FailedMessageIds;
        foreach (var filenameAndMessageId in successfulFileNamesAndFailedMessageIds.Value.SuccessfulFiles)
        {
            var entity = await _receiptCommandRepository.GetReceiptById(filenameAndMessageId.Filename, cancellationToken);
            if (entity is null)
            {
                Console.WriteLine($"ERROR: entity doesnt exist id: {filenameAndMessageId.Filename}");
                failedMessageIdsWithReason.Add(Error.Failure(filenameAndMessageId.MessageId, $"ERROR: entity doesnt exist id: {filenameAndMessageId.Filename}"));
                continue;
            }

            var fileContents = await _fileUploadRepository.GetContentsOfFile(filenameAndMessageId.Filename, cancellationToken);
            var fileHandler = SelectHandler(fileContents);
            var fileHandlerResult = await fileHandler.Handle(fileContents, filenameAndMessageId.MessageId, entity.UserId, filenameAndMessageId.Filename, cancellationToken);
            if (fileHandlerResult.HasError)
            {
                failedMessageIdsWithReason.Add(fileHandlerResult.Error!);
                continue;
            }

            entity.UpdateState((int)ReceiptState.Pending);
            await _receiptCommandRepository.UpdateReceipt(entity, cancellationToken);

            await _messageQueueRepository.DeleteMessage(filenameAndMessageId.QueueMessageId, cancellationToken);
            successfulMessageIds.Add(filenameAndMessageId.MessageId);
        }

        if (await _receiptCommandRepository.GetNumberOfReceiptsLeftToProcess(cancellationToken) == 0)
        {
            _pollingController.DisablePolling();
        }

        return new MessageQueueResult
        {
            SuccessfullyProcessedFileIds = successfulMessageIds,
            FailedProcessedFileIds = failedMessageIdsWithReason,
        };
    }

    private IHandler SelectHandler(string fileContents)
    {
        var fileWithVersionOnly = JsonSerializer.Deserialize<VersionNumberObject>(fileContents);
        return fileWithVersionOnly?.VersionNumber switch
        {
            1 => new HandleV1(_receiptCommandRepository),
            _ => throw new InvalidOperationException(),
        };
    }
}
