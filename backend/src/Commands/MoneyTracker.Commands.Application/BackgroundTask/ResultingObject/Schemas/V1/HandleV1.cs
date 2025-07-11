
using System.Text.Json;
using MoneyTracker.Commands.Domain.Entities.Transaction;
using MoneyTracker.Commands.Domain.Repositories;
using MoneyTracker.Common.Result;

namespace MoneyTracker.Commands.Application.BackgroundTask.ResultingObject.Schemas.V1;
public class HandleV1 : IHandler
{
    private readonly IReceiptCommandRepository _receiptCommandRepository;

    public HandleV1(IReceiptCommandRepository receiptCommandRepository)
    {
        _receiptCommandRepository = receiptCommandRepository;
    }

    public async Task<Result> Handle(string fileContents, string messageId, int userId, string filename, CancellationToken cancellationToken)
    {
        var infoFromReceipt = JsonSerializer.Deserialize<TemporaryTransactionObject>(fileContents);
        if (infoFromReceipt is null)
        {
            Console.WriteLine($"ERROR: object is wrong??");
            return Error.Failure(messageId, $"ERROR: object is wrong: {fileContents}");
        }

        var temporaryTransaction = new TemporaryTransactionEntity
        {
            UserId = userId,
            Filename = filename,
            Amount = infoFromReceipt.Data.Value,
            CategoryId = null,
            DatePaid = null,
            PayeeId = null,
            PayerId = null,
        };
        await _receiptCommandRepository.CreateTemporaryTransaction(temporaryTransaction, cancellationToken);

        return Result.Success();
    }
}
