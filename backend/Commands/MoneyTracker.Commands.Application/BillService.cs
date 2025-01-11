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
    private readonly IUserCommandRepository _userRepository;
    private readonly IUserService _userService;

    public BillService(IBillCommandRepository dbService,
        IAccountCommandRepository accountDatabase,
        IIdGenerator idGenerator,
        IFrequencyCalculation frequencyCalculation,
        IMonthDayCalculator monthDayCalculator,
        ICategoryCommandRepository categoryDatabase,
        IUserCommandRepository userRepository,
        IUserService userService
        )
    {
        _dbService = dbService;
        _accountDatabase = accountDatabase;
        _idGenerator = idGenerator;
        _frequencyCalculation = frequencyCalculation;
        _monthDayCalculator = monthDayCalculator;
        _categoryDatabase = categoryDatabase;
        _userRepository = userRepository;
        _userService = userService;
    }

    public async Task<Result> AddBill(string token, NewBillRequest newBill)
    {
        var userResult = await _userService.GetUserFromToken(token);
        if (!userResult.IsSuccess)
            return userResult;

        var user = userResult.Value;
        if (!await _accountDatabase.IsAccountOwnedByUser(user, newBill.PayerId))
        {
            return Result.Failure(Error.Validation("BillService.AddBill", "Payer account not found"));
        }
        if (!await _accountDatabase.IsValidAccount(newBill.PayeeId))
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

    public async Task EditBill(string token, EditBillRequest editBill)
    {
        var userAuth = await _userRepository.GetUserAuthFromToken(token);
        if (userAuth == null)
            throw new InvalidDataException("Token not found");
        userAuth.CheckValidation();

        var user = new AuthenticatedUser(userAuth.User.Id);
        if (editBill.PayeeId == null && editBill.Amount == null &&
            editBill.Amount == null && editBill.NextDueDate == null &&
            editBill.Frequency == null && editBill.CategoryId == null &&
            editBill.PayerId == null)
        {
            throw new InvalidDataException("Must have at least one non-null value");
        }

        if (!await _dbService.IsBillAssociatedWithUser(user, editBill.Id))
        {
            throw new InvalidDataException("Bill not found");
        }
        if (editBill.PayerId != null &&
            !await _accountDatabase.IsAccountOwnedByUser(user, (int)editBill.PayerId))
        {
            throw new InvalidDataException("Payer account not found");
        }
        if (editBill.PayeeId != null &&
            !await _accountDatabase.IsValidAccount((int)editBill.PayeeId))
        {
            throw new InvalidDataException("Payee account not found");
        }
        if (editBill.Frequency != null &&
            !_frequencyCalculation.DoesFrequencyExist(editBill.Frequency))
        {
            throw new InvalidDataException("Invalid frequency");
        }
        if (editBill.CategoryId != null && !await _categoryDatabase.DoesCategoryExist((int)editBill.CategoryId))
        {
            throw new InvalidDataException("Invalid category");
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
    }

    public async Task DeleteBill(string token, DeleteBillRequest deleteBill)
    {
        var userAuth = await _userRepository.GetUserAuthFromToken(token);
        if (userAuth == null)
            throw new InvalidDataException("Token not found");
        userAuth.CheckValidation();

        var user = new AuthenticatedUser(userAuth.User.Id);
        if (!await _dbService.IsBillAssociatedWithUser(user, deleteBill.Id))
        {
            throw new InvalidDataException("Bill not found");
        }

        await _dbService.DeleteBill(deleteBill.Id);
    }

    public async Task SkipOccurence(string token, SkipBillOccurrenceRequest skipBillDTO)
    {
        var userAuth = await _userRepository.GetUserAuthFromToken(token);
        if (userAuth == null)
            throw new InvalidDataException("Token not found");
        userAuth.CheckValidation();

        var user = new AuthenticatedUser(userAuth.User.Id);
        if (!await _dbService.IsBillAssociatedWithUser(user, skipBillDTO.Id))
        {
            throw new InvalidDataException("Bill not found");
        }

        var bill = await _dbService.GetBillById(skipBillDTO.Id);
        if (bill == null)
        {
            throw new InvalidDataException("Unexpected database error - bill not found"); // log this
        }
        var newDueDate = _frequencyCalculation.CalculateNextDueDate(bill.Frequency, bill.MonthDay, skipBillDTO.SkipDatePastThisDate);

        var editBill = new EditBillEntity(skipBillDTO.Id, nextDueDate: newDueDate);
        await _dbService.EditBill(editBill);
    }
}
