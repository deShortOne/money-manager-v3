
using System.Text;
using System.Text.Json;
using MoneyTracker.Commands.Application.AWS;
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

    public MessageQueueService(IMessageQueueRepository messageQueueRepository,
        IReceiptCommandRepository receiptCommandRepository,
        IFileUploadRepository fileUploadRepository)
    {
        _messageQueueRepository = messageQueueRepository;
        _receiptCommandRepository = receiptCommandRepository;
        _fileUploadRepository = fileUploadRepository;
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

            Console.WriteLine($"There are {body.Records.Count} records");

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
                    Amount = infoFromReceipt.Value,
                    CategoryId = null,
                    DatePaid = null,
                    PayeeId = null,
                    PayerId = null,
                };
                await _receiptCommandRepository.CreateTemporaryTransaction(entity.UserId, temporaryTransaction);

                entity.UpdateState((int)ReceiptState.Finished);
                await _receiptCommandRepository.UpdateReceipt(entity);
            }
        }
        Console.WriteLine("============");
    }
}
