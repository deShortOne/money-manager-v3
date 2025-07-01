using Microsoft.AspNetCore.Http;
using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Commands.Domain.Entities.MessageQueuePolling;
using MoneyTracker.Commands.Domain.Entities.Receipt;
using MoneyTracker.Commands.Domain.Entities.Transaction;
using MoneyTracker.Commands.Domain.Handlers;
using MoneyTracker.Commands.Domain.Repositories;
using MoneyTracker.Common.Result;
using MoneyTracker.Common.Utilities.DateTimeUtil;
using MoneyTracker.Common.Utilities.IdGeneratorUtil;
using MoneyTracker.Contracts.Requests.Transaction;
using MoneyTracker.PlatformService.Domain;
using MoneyTracker.PlatformService.DTOs;

namespace MoneyTracker.Commands.Application;
public class RegisterService : IRegisterService
{
    private readonly IRegisterCommandRepository _registerDb;
    private readonly IAccountCommandRepository _accountDb;
    private readonly IIdGenerator _idGenerator;
    private readonly IUserService _userService;
    private readonly IAccountService _accountService;
    private readonly ICategoryService _categoryService;
    private readonly IMessageBusClient _messageBus;
    private readonly IFileUploadRepository _fileUploadRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IReceiptCommandRepository _receiptCommandRepository;
    private readonly IPollingController _pollingController;

    public RegisterService(IRegisterCommandRepository registerDb,
        IAccountCommandRepository accountDb,
        IIdGenerator idGenerator,
        IUserService userService,
        IAccountService accountService,
        ICategoryService categoryService,
        IMessageBusClient messageBus,
        IFileUploadRepository fileUploadRepository,
        IDateTimeProvider dateTimeProvider,
        IReceiptCommandRepository receiptCommandRepository,
        IPollingController pollingController
        )
    {
        _registerDb = registerDb;
        _accountDb = accountDb;
        _idGenerator = idGenerator;
        _userService = userService;
        _accountService = accountService;
        _categoryService = categoryService;
        _messageBus = messageBus;
        _fileUploadRepository = fileUploadRepository;
        _dateTimeProvider = dateTimeProvider;
        _receiptCommandRepository = receiptCommandRepository;
        _pollingController = pollingController;
    }

    public async Task<Result> AddTransaction(string token, NewTransactionRequest newTransaction)
    {
        var userResult = await _userService.GetUserFromToken(token);
        if (userResult.HasError)
            return userResult;

        var user = userResult.Value;

        var payerAccount = await _accountDb.GetAccountUserEntity(newTransaction.PayerId);
        if (payerAccount == null) // to be logged differently
        {
            return Error.Validation("RegisterService.AddTransaction", "Payer account not found");
        }
        if (!payerAccount.UserOwnsAccount || payerAccount.UserId != user.Id)
        {
            return Error.Validation("RegisterService.AddTransaction", "Payer account not found");
        }

        var payeeAccount = await _accountDb.GetAccountUserEntity(newTransaction.PayeeId);
        if (payeeAccount == null) // to be logged differently
        {
            return Error.Validation("RegisterService.AddTransaction", "Payee account not found");
        }
        if (payeeAccount.UserId != user.Id)
        {
            return Error.Validation("RegisterService.AddTransaction", "Payee account not found");
        }

        if (!await _categoryService.DoesCategoryExist(newTransaction.CategoryId))
        {
            return Error.Validation("RegisterService.AddTransaction", "Category not found");
        }

        var newTransactionId = _idGenerator.NewInt(await _registerDb.GetLastTransactionId());

        var dtoToDb = new TransactionEntity(newTransactionId,
            newTransaction.PayeeId,
            newTransaction.Amount,
            newTransaction.DatePaid,
            newTransaction.CategoryId,
            newTransaction.PayerId);

        await _registerDb.AddTransaction(dtoToDb);

        await _messageBus.PublishEvent(new EventUpdate(user, DataTypes.Register), CancellationToken.None);

        return Result.Success();
    }

    public async Task<Result> EditTransaction(string token, EditTransactionRequest editTransaction)
    {
        var userResult = await _userService.GetUserFromToken(token);
        if (userResult.HasError)
            return userResult;

        var user = userResult.Value;
        if (!await DoesUserOwnTransaction(user, editTransaction.Id))
        {
            return Error.Validation("RegisterService.EditTransaction", "Transaction not found");
        }
        if (editTransaction.PayerId != null)
        {
            var payerAccount = await _accountDb.GetAccountUserEntity((int)editTransaction.PayerId, user.Id);
            if (payerAccount == null)
            {
                return Error.Validation("RegisterService.EditTransaction", "Payer account not found");
            }
            if (!payerAccount.UserOwnsAccount)
                return Error.Validation("RegisterService.EditTransaction", "Payer account not found");
        }

        var dtoToDb = new EditTransactionEntity(editTransaction.Id, editTransaction.PayeeId, editTransaction.Amount,
            editTransaction.DatePaid, editTransaction.CategoryId, editTransaction.PayerId);

        await _registerDb.EditTransaction(dtoToDb);

        await _messageBus.PublishEvent(new EventUpdate(user, DataTypes.Register), CancellationToken.None);

        return Result.Success();
    }

    public async Task<Result> DeleteTransaction(string token, DeleteTransactionRequest deleteTransaction)
    {
        var userResult = await _userService.GetUserFromToken(token);
        if (userResult.HasError)
            return userResult;

        var user = userResult.Value;
        if (!await DoesUserOwnTransaction(user, deleteTransaction.Id))
        {
            return Error.Validation("RegisterService.DeleteTransaction", "Transaction not found");
        }

        await _registerDb.DeleteTransaction(deleteTransaction.Id);

        await _messageBus.PublishEvent(new EventUpdate(user, DataTypes.Register), CancellationToken.None);

        return Result.Success();
    }

    public async Task<bool> DoesUserOwnTransaction(AuthenticatedUser user, int transactionId)
    {
        var transaction = await _registerDb.GetTransaction(transactionId);
        if (transaction == null)
            return false;

        return await _accountService.DoesUserOwnAccount(user, transaction.PayerId);
    }

    public async Task<Result> CreateTransactionFromReceipt(string token, IFormFile createTransactionFromReceipt)
    {
        var userResult = await _userService.GetUserFromToken(token);
        if (userResult.HasError)
            return userResult;

        var fileNameBrokenUp = createTransactionFromReceipt.FileName.Split(".");
        var fileNameExtension = fileNameBrokenUp[fileNameBrokenUp.Length - 1];
        var fileName = string.Join(".", fileNameBrokenUp.Take(fileNameBrokenUp.Length - 1));
        var id = $"{fileName}-{_dateTimeProvider.Now.ToString("yyyyMMdd-HHmmss")}.{fileNameExtension}";

        var fileUploadUrl = await _fileUploadRepository.UploadAsync(createTransactionFromReceipt, id);

        await _receiptCommandRepository.AddReceipt(new ReceiptEntity(id, userResult.Value.Id, createTransactionFromReceipt.FileName, fileUploadUrl, (int)ReceiptState.Processing));

        _pollingController.EnablePolling();

        return Result.Success();
    }
}
