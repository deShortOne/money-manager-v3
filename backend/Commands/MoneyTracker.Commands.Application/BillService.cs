using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Commands.Domain.Entities.Bill;
using MoneyTracker.Commands.Domain.Handlers;
using MoneyTracker.Commands.Domain.Repositories;
using MoneyTracker.Common.Result;
using MoneyTracker.Common.Utilities.CalculationUtil;
using MoneyTracker.Common.Utilities.IdGeneratorUtil;
using MoneyTracker.Contracts.Requests.Bill;
using MoneyTracker.PlatformService.Domain;
using MoneyTracker.PlatformService.DTOs;

namespace MoneyTracker.Commands.Application;
public class BillService : IBillService
{
    private readonly IBillCommandRepository _dbService;
    private readonly IAccountCommandRepository _accountDatabase;
    private readonly IIdGenerator _idGenerator;
    private readonly IFrequencyCalculation _frequencyCalculation;
    private readonly IMonthDayCalculator _monthDayCalculator;
    private readonly ICategoryService _categoryService;
    private readonly IUserService _userService;
    private readonly IAccountService _accountService;
    private readonly IMessageBusClient _messageBus;

    public BillService(IBillCommandRepository dbService,
        IAccountCommandRepository accountDatabase,
        IIdGenerator idGenerator,
        IFrequencyCalculation frequencyCalculation,
        IMonthDayCalculator monthDayCalculator,
        ICategoryService categoryService,
        IUserService userService,
        IAccountService accountService,
        IMessageBusClient messageBus
        )
    {
        _dbService = dbService;
        _accountDatabase = accountDatabase;
        _idGenerator = idGenerator;
        _frequencyCalculation = frequencyCalculation;
        _monthDayCalculator = monthDayCalculator;
        _categoryService = categoryService;
        _userService = userService;
        _accountService = accountService;
        _messageBus = messageBus;
    }

    public async Task<Result> AddBill(string token, NewBillRequest newBill)
    {
        var userResult = await _userService.GetUserFromToken(token);
        if (!userResult.IsSuccess)
            return userResult;

        var user = userResult.Value;
        if (newBill.Amount < 0)
        {
            return Error.Validation("BillService.AddBill", "Amount must be a positive number");
        }

        if (!await _accountService.DoesUserOwnAccount(user, newBill.PayerId))
        {
            return Error.Validation("BillService.AddBill", "Payer account not found");
        }
        var payeeAccount = await _accountDatabase.GetAccountUserEntity(newBill.PayeeId);
        if (payeeAccount == null || payeeAccount.UserId != user.Id)
        {
            return Error.Validation("BillService.AddBill", "Payee account not found");
        }
        if (!_frequencyCalculation.DoesFrequencyExist(newBill.Frequency))
        {
            return Error.Validation("BillService.AddBill", "Frequency type not found");
        }
        if (!await _categoryService.DoesCategoryExist(newBill.CategoryId))
        {
            return Error.Validation("BillService.AddBill", "Category not found");
        }

        var dtoToDb = new BillEntity(
            _idGenerator.NewInt(await _dbService.GetLastId()),
            newBill.PayeeId,
            newBill.Amount,
            newBill.NextDueDate,
            _monthDayCalculator.Calculate(newBill.NextDueDate),
            newBill.Frequency,
            newBill.CategoryId,
            newBill.PayerId
        );
        await _dbService.AddBill(dtoToDb);

        await _messageBus.PublishEvent(new EventUpdate(user, DataTypes.Bill), CancellationToken.None);

        return Result.Success();
    }

    public async Task<Result> EditBill(string token, EditBillRequest editBill)
    {
        var userResult = await _userService.GetUserFromToken(token);
        if (!userResult.IsSuccess)
            return userResult;

        var user = userResult.Value;
        if (editBill.PayeeId == null && editBill.Amount == null &&
            editBill.Amount == null && editBill.NextDueDate == null &&
            editBill.Frequency == null && editBill.CategoryId == null &&
            editBill.PayerId == null)
        {
            return Error.Validation("BillService.EditBill", "Must have at least one non-null value");
        }

        var getBillIfOwnedByUser = await GetBillIfOwnedByUser(editBill.Id, user);
        if (getBillIfOwnedByUser == null)
        {
            return Error.Validation("BillService.EditBill", "Bill not found");
        }

        if (editBill.PayerId != null)
        {
            if (!await _accountService.DoesUserOwnAccount(user, (int)editBill.PayerId))
                return Error.Validation("BillService.EditBill", "Payer account not found");
        }
        if (editBill.PayeeId != null)
        {
            var payeeAccount = await _accountDatabase.GetAccountUserEntity((int)editBill.PayeeId, user.Id);
            if (payeeAccount == null)
            {
                return Error.Validation("BillService.EditBill", "Payee account not found");
            }
        }
        if (editBill.Frequency != null &&
            !_frequencyCalculation.DoesFrequencyExist(editBill.Frequency))
        {
            return Error.Validation("BillService.EditBill", "Frequency type not found");
        }
        if (editBill.CategoryId != null && !await _categoryService.DoesCategoryExist((int)editBill.CategoryId))
        {
            return Error.Validation("BillService.EditBill", "Category not found");
        }

        int? monthDay = null;
        if (editBill.NextDueDate != null)
        {
            monthDay = _monthDayCalculator.Calculate((DateOnly)editBill.NextDueDate);
        }

        var dtoToDb = new EditBillEntity(
            editBill.Id,
            editBill.PayeeId,
            editBill.Amount,
            editBill.NextDueDate,
            monthDay,
            editBill.Frequency,
            editBill.CategoryId,
            editBill.PayerId
        );
        await _dbService.EditBill(dtoToDb);

        await _messageBus.PublishEvent(new EventUpdate(user, DataTypes.Bill), CancellationToken.None);

        return Result.Success();
    }

    public async Task<Result> DeleteBill(string token, DeleteBillRequest deleteBill)
    {
        var userResult = await _userService.GetUserFromToken(token);
        if (!userResult.IsSuccess)
            return userResult;

        var user = userResult.Value;
        var doesUserOwnBill = await GetBillIfOwnedByUser(deleteBill.Id, user);
        if (doesUserOwnBill == null)
        {
            return Error.Validation("BillService.DeleteBill", "Bill not found");
        }

        await _dbService.DeleteBill(deleteBill.Id);

        await _messageBus.PublishEvent(new EventUpdate(user, DataTypes.Bill), CancellationToken.None);

        return Result.Success();
    }

    public async Task<Result> SkipOccurence(string token, SkipBillOccurrenceRequest skipBillDTO)
    {
        var userResult = await _userService.GetUserFromToken(token);
        if (!userResult.IsSuccess)
            return userResult;

        var user = userResult.Value;
        var getBillIfOwnedByUser = await GetBillIfOwnedByUser(skipBillDTO.Id, user);
        if (getBillIfOwnedByUser == null)
        {
            return Error.Validation("BillService.SkipOccurence", "Bill not found");
        }

        var newDueDate = _frequencyCalculation.CalculateNextDueDate(getBillIfOwnedByUser.Frequency, getBillIfOwnedByUser.MonthDay, skipBillDTO.SkipDatePastThisDate);

        var editBill = new EditBillEntity(skipBillDTO.Id, nextDueDate: newDueDate);
        await _dbService.EditBill(editBill);

        await _messageBus.PublishEvent(new EventUpdate(user, DataTypes.Bill), CancellationToken.None);

        return Result.Success();
    }

    private async Task<BillEntity?> GetBillIfOwnedByUser(int billId, AuthenticatedUser user)
    {
        var bill = await _dbService.GetBillById(billId);
        if (bill == null)
        {
            return null;
        }
        var billsPayerAccount = await _accountDatabase.GetAccountUserEntity(bill.PayerId, user.Id);
        if (billsPayerAccount == null || !billsPayerAccount.UserOwnsAccount)
        {
            return null;
        }
        return bill;
    }
}
