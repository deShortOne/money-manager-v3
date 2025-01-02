using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Authentication.Interfaces;
using MoneyTracker.Commands.Domain.Entities.Bill;
using MoneyTracker.Commands.Domain.Handlers;
using MoneyTracker.Commands.Domain.Repositories;
using MoneyTracker.Common.Utilities.CalculationUtil;
using MoneyTracker.Common.Utilities.DateTimeUtil;
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

    public BillService(IBillCommandRepository dbService,
        IAccountCommandRepository accountDatabase,
        IIdGenerator idGenerator,
        IFrequencyCalculation frequencyCalculation,
        IMonthDayCalculator monthDayCalculator,
        ICategoryCommandRepository categoryDatabase,
        IUserCommandRepository userRepository)
    {
        _dbService = dbService;
        _accountDatabase = accountDatabase;
        _idGenerator = idGenerator;
        _frequencyCalculation = frequencyCalculation;
        _monthDayCalculator = monthDayCalculator;
        _categoryDatabase = categoryDatabase;
        _userRepository = userRepository;
    }

    public async Task AddBill(string token, NewBillRequest newBill)
    {
        var userAuth = await _userRepository.GetUserAuthFromToken(token);
        if (userAuth == null)
            throw new InvalidDataException("Token not found");
        userAuth.ThrowIfInvalid();

        var user = new AuthenticatedUser(userAuth.User.Id);
        if (!await _accountDatabase.IsAccountOwnedByUser(user, newBill.Payer))
        {
            throw new InvalidDataException("Account not found");
        }
        if (!_frequencyCalculation.DoesFrequencyExist(newBill.Frequency))
        {
            throw new InvalidDataException("Invalid frequency");
        }
        if (!await _categoryDatabase.DoesCategoryExist(newBill.CategoryId))
        {
            throw new InvalidDataException("Invalid category");
        }

        var dtoToDb = new BillEntity(
            _idGenerator.NewInt(await _dbService.GetLastId()),
            newBill.Payee,
            newBill.Amount,
            newBill.NextDueDate,
            _monthDayCalculator.Calculate(newBill.NextDueDate),
            newBill.Frequency,
            newBill.CategoryId,
            newBill.Payer
        );
        await _dbService.AddBill(dtoToDb);
    }

    public async Task EditBill(string token, EditBillRequest editBill)
    {
        var userAuth = await _userRepository.GetUserAuthFromToken(token);
        if (userAuth == null)
            throw new InvalidDataException("Token not found");
        userAuth.ThrowIfInvalid();

        var user = new AuthenticatedUser(userAuth.User.Id);
        if (editBill.Payee == null && editBill.Amount == null &&
            editBill.Amount == null && editBill.NextDueDate == null &&
            editBill.Frequency == null && editBill.Category == null &&
            editBill.AccountId == null)
        {
            throw new InvalidDataException("Must have at least one non-null value");
        }

        if (!await _dbService.IsBillAssociatedWithUser(user, editBill.Id))
        {
            throw new InvalidDataException("Bill not found");
        }
        if (editBill.AccountId != null &&
            !await _accountDatabase.IsAccountOwnedByUser(user, (int)editBill.AccountId))
        {
            throw new InvalidDataException("Account not found");
        }
        if (editBill.Frequency != null &&
            !_frequencyCalculation.DoesFrequencyExist(editBill.Frequency))
        {
            throw new InvalidDataException("Invalid frequency");
        }
        if (editBill.Category != null && !await _categoryDatabase.DoesCategoryExist((int)editBill.Category))
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
            editBill.Payee,
            editBill.Amount,
            editBill.NextDueDate,
            monthDay,
            editBill.Frequency,
            editBill.Category,
            editBill.AccountId
        );
        await _dbService.EditBill(dtoToDb);
    }

    public async Task DeleteBill(string token, DeleteBillRequest deleteBill)
    {
        var userAuth = await _userRepository.GetUserAuthFromToken(token);
        if (userAuth == null)
            throw new InvalidDataException("Token not found");
        userAuth.ThrowIfInvalid();

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
        userAuth.ThrowIfInvalid();

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
