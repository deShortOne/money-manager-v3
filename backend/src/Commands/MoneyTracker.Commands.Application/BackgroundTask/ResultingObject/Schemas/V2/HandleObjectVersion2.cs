
using System.Text.Json;
using MoneyTracker.Commands.Domain.Entities.Account;
using MoneyTracker.Commands.Domain.Entities.Transaction;
using MoneyTracker.Commands.Domain.Repositories;
using MoneyTracker.Common.Result;

namespace MoneyTracker.Commands.Application.BackgroundTask.ResultingObject.Schemas.V2;
public class HandleObjectVersion2 : IHandler
{
    private readonly IReceiptCommandRepository _receiptCommandRepository;
    private readonly IAccountCommandRepository _accountCommandRepository;
    private readonly ICategoryCommandRepository _categoryCommandRepository;

    public HandleObjectVersion2(IReceiptCommandRepository receiptCommandRepository,
        IAccountCommandRepository accountCommandRepository,
        ICategoryCommandRepository categoryCommandRepository)
    {
        _receiptCommandRepository = receiptCommandRepository;
        _accountCommandRepository = accountCommandRepository;
        _categoryCommandRepository = categoryCommandRepository;
    }

    public int VersionNumber => 2;

    public async Task<Result> Handle(string fileContents, string messageId, int userId, string filename, CancellationToken cancellationToken)
    {
        var infoFromReceipt = JsonSerializer.Deserialize<TemporaryTransactionObject>(fileContents);
        if (infoFromReceipt is null)
        {
            Console.WriteLine($"ERROR: object is wrong??");
            return Error.Failure(messageId, $"ERROR: object is wrong: {fileContents}");
        }

        var transactionObject = infoFromReceipt.Data;
        if (transactionObject is null)
            return Error.Failure(messageId, $"ERROR: object does not contain data: {fileContents}");

        AccountUserEntity? payee = null;
        if (transactionObject.PayeeName is not null)
        {
            payee = await _accountCommandRepository.GetAccountUserEntity(transactionObject.PayeeName, userId, cancellationToken);
            if (payee is null)
            {
                return Error.NotFound(messageId, $"ERROR: payee cannot be found: {transactionObject.PayeeName}");
            }
        }

        AccountUserEntity? payer = null;
        if (transactionObject.PayeeName is not null)
        {
            payer = await _accountCommandRepository.GetAccountUserEntity(transactionObject.PayerName, userId, cancellationToken);
            if (payer is null)
            {
                return Error.NotFound(messageId, $"ERROR: payer cannot be found: {transactionObject.PayerName}");
            }
        }

        var temporaryTransaction = new TemporaryTransactionEntity
        {
            UserId = userId,
            Filename = filename,
            Amount = transactionObject.Amount,
            CategoryId = null,
            DatePaid = transactionObject.DatePaid,
            PayeeId = payee?.Id,
            PayerId = payer?.Id,
        };
        await _receiptCommandRepository.CreateTemporaryTransaction(temporaryTransaction, cancellationToken);

        return Result.Success();
    }
}
