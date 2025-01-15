using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Commands.Domain.Entities.Bill;
using MoneyTracker.Commands.Domain.Handlers;
using MoneyTracker.Commands.Domain.Repositories;
using MoneyTracker.Common.Result;
using MoneyTracker.Common.Utilities.CalculationUtil;
using MoneyTracker.Common.Utilities.IdGeneratorUtil;
using MoneyTracker.Contracts.Requests.Bill;

namespace MoneyTracker.Commands.Application;
public class BillService : IBillService
{
    private readonly IBillCommandRepository _dbService;
    private readonly IAccountCommandRepository _accountDatabase;
    private readonly IIdGenerator _idGenerator;
    private readonly IFrequencyCalculation _frequencyCalculation;
    private readonly IMonthDayCalculator _monthDayCalculator;
    private readonly ICategoryCommandRepository _categoryDatabase;
    private readonly IUserService _userService;

    public BillService(IBillCommandRepository dbService,
        IAccountCommandRepository accountDatabase,
        IIdGenerator idGenerator,
        IFrequencyCalculation frequencyCalculation,
        IMonthDayCalculator monthDayCalculator,
        ICategoryCommandRepository categoryDatabase,
        IUserService userService
        )
    {
        _dbService = dbService;
        _accountDatabase = accountDatabase;
        _idGenerator = idGenerator;
        _frequencyCalculation = frequencyCalculation;
        _monthDayCalculator = monthDayCalculator;
        _categoryDatabase = categoryDatabase;
        _userService = userService;
    }

    public async Task<Result> AddBill(string token, NewBillRequest newBill)
    {
        var userResult = await _userService.GetUserFromToken(token);
        if (!userResult.IsSuccess)
            return userResult;

        var user = userResult.Value;
        if (newBill.Amount < 0)
        {
            return Result.Failure(Error.Validation("BillService.AddBill", "Amount must be a positive number"));
        }

        var payerAccount = await _accountDatabase.GetAccountById(newBill.PayerId);
        if (payerAccount == null) // to be logged differently
        {
            return Result.Failure(Error.Validation("BillService.AddBill", "Payer account not found"));
        }
        if (payerAccount.UserId != user.Id)
        {
            return Result.Failure(Error.Validation("BillService.AddBill", "Payer account not found"));
        }

        var payeeAccount = await _accountDatabase.GetAccountById(newBill.PayeeId);
        if (payeeAccount == null)
        {
            return Result.Failure(Error.Validation("BillService.AddBill", "Payee account not found"));
        }
        if (!_frequencyCalculation.DoesFrequencyExist(newBill.Frequency))
        {
            return Result.Failure(Error.Validation("BillService.AddBill", "Frequency type not found"));
        }
        if (!await _categoryDatabase.DoesCategoryExist(newBill.CategoryId))
        {
            return Result.Failure(Error.Validation("BillService.AddBill", "Category not found"));
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
            return Result.Failure(Error.Validation("BillService.EditBill", "Must have at least one non-null value"));
        }

        if (!await _dbService.IsBillAssociatedWithUser(user, editBill.Id))
        {
            return Result.Failure(Error.Validation("BillService.EditBill", "Bill not found"));
        }

        if (editBill.PayerId != null)
        {
            var payerAccount = await _accountDatabase.GetAccountById((int)editBill.PayerId);
            if (payerAccount == null) // to be logged differently
            {
                return Result.Failure(Error.Validation("BillService.EditBill", "Payer account not found"));
            }
            if (payerAccount.UserId != user.Id)
            {
                return Result.Failure(Error.Validation("BillService.EditBill", "Payer account not found"));
            }
        }
        if (editBill.PayeeId != null)
        {
            var payeeAccount = await _accountDatabase.GetAccountById((int)editBill.PayeeId);
            if (payeeAccount == null)
            {
                return Result.Failure(Error.Validation("BillService.EditBill", "Payee account not found"));
            }
        }
        if (editBill.Frequency != null &&
            !_frequencyCalculation.DoesFrequencyExist(editBill.Frequency))
        {
            return Result.Failure(Error.Validation("BillService.EditBill", "Frequency type not found"));
        }
        if (editBill.CategoryId != null && !await _categoryDatabase.DoesCategoryExist((int)editBill.CategoryId))
        {
            return Result.Failure(Error.Validation("BillService.EditBill", "Category not found"));
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

        return Result.Success();
    }

    public async Task<Result> DeleteBill(string token, DeleteBillRequest deleteBill)
    {
        var userResult = await _userService.GetUserFromToken(token);
        if (!userResult.IsSuccess)
            return userResult;

        var user = userResult.Value;
        if (!await _dbService.IsBillAssociatedWithUser(user, deleteBill.Id))
        {
            return Result.Failure(Error.Validation("BillService.DeleteBill", "Bill not found"));
        }

        await _dbService.DeleteBill(deleteBill.Id);

        return Result.Success();
    }

    public async Task<Result> SkipOccurence(string token, SkipBillOccurrenceRequest skipBillDTO)
    {
        var userResult = await _userService.GetUserFromToken(token);
        if (!userResult.IsSuccess)
            return userResult;

        var user = userResult.Value;
        if (!await _dbService.IsBillAssociatedWithUser(user, skipBillDTO.Id))
        {
            return Result.Failure(Error.Validation("BillService.SkipOccurence", "Bill not found"));
        }

        var bill = await _dbService.GetBillById(skipBillDTO.Id);
        if (bill == null)
        {
            return Result.Failure(Error.Failure("BillService.SkipOccurence", "Bill not found"));
        }
        var newDueDate = _frequencyCalculation.CalculateNextDueDate(bill.Frequency, bill.MonthDay, skipBillDTO.SkipDatePastThisDate);

        var editBill = new EditBillEntity(skipBillDTO.Id, nextDueDate: newDueDate);
        await _dbService.EditBill(editBill);

        return Result.Success();
    }
}
