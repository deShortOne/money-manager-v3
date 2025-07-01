
using System.Text;
using System.Text.Json;
using MoneyTracker.Commands.Application.AWS;
using MoneyTracker.Commands.Domain.Entities.MessageQueuePolling;
using MoneyTracker.Commands.Domain.Entities.Receipt;
using MoneyTracker.Commands.Domain.Entities.Transaction;
using MoneyTracker.Commands.Domain.Handlers;
using MoneyTracker.Commands.Domain.Repositories;

namespace MoneyTracker.Commands.Application;
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

            var body = JsonSerializer.Deserialize<MessageBody>(message.Body);
            if (body is null)
            {
                Console.WriteLine($"ERROR: CANNOT PARSE BODY: {message.Body}");
                continue;
            }

            foreach (var record in body.Records)
            {
                var filename = record.S3.S3Object.Key;
                var entity = await _receiptCommandRepository.GetReceiptById(filename);
                if (entity is null)
                {
                    Console.WriteLine($"ERROR: entity doesnt exist id: {filename}");
                    continue;
                }

                var fileContents = await _fileUploadRepository.GetContentsOfFile(filename, cancellationToken);

                var infoFromReceipt = JsonSerializer.Deserialize<TemporaryTransactionObject>(fileContents);
                if (infoFromReceipt is null)
                {
                    Console.WriteLine($"ERROR: object is wrong??");
                    continue;
                }

                var temporaryTransaction = new TemporaryTransactionEntity
                {
                    UserId = entity.UserId,
                    Amount = infoFromReceipt.Value,
                    CategoryId = null,
                    DatePaid = null,
                    PayeeId = null,
                    PayerId = null,
                };
                await _receiptCommandRepository.CreateTemporaryTransaction(temporaryTransaction);

                entity.UpdateState((int)ReceiptState.Pending);
                await _receiptCommandRepository.UpdateReceipt(entity);
            }
            if (body.Records.Count == 0)
            {
                Console.WriteLine($"ERROR: There are 0 records inside message {message.MessageId}, with information {message.Body}");
            }
            else
            {
                await _messageQueueRepository.DeleteMessage(message.ReceiptHandle, cancellationToken);
            }
        }

        if (await _receiptCommandRepository.GetNumberOfReceiptsLeftToProcess() == 0)
        {
            _pollingController.DisablePolling();
        }
    }
}
