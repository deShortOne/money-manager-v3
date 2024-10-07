using MoneyTracker.Calculation.Bill;
using MoneyTracker.Data.Postgres;
using MoneyTracker.Shared.Core;
using MoneyTracker.Shared.Data;
using MoneyTracker.Shared.DateManager;
using MoneyTracker.Shared.Models.ControllerToService.Bill;
using MoneyTracker.Shared.Models.RepositoryToService.Bill;
using MoneyTracker.Shared.Models.ServiceToController.Bill;
using MoneyTracker.Shared.Models.ServiceToRepository.Bill;
using MoneyTracker.Shared.Shared;

namespace MoneyTracker.Core;
public class BillService : IBillService
{
    private readonly IBillDatabase _dbService;
    private readonly IDateProvider _dateProvider;
    private readonly IUserAuthenticationService _userAuthService;
    private readonly IAccountDatabase _accountDatabase;
    private readonly IIdGenerator _idGenerator;
    private readonly IFrequencyCalculation _frequencyCalculation;
    private readonly IMonthDayCalculator _monthDayCalculator;

    public BillService(IBillDatabase dbService,
        IDateProvider dateProvider,
        IUserAuthenticationService userAuthService,
        IAccountDatabase accountDatabase,
        IIdGenerator idGenerator,
        IFrequencyCalculation frequencyCalculation,
        IMonthDayCalculator monthDayCalculator)
    {
        _dbService = dbService;
        _dateProvider = dateProvider;
        _userAuthService = userAuthService;
        _accountDatabase = accountDatabase;
        _idGenerator = idGenerator;
        _frequencyCalculation = frequencyCalculation;
        _monthDayCalculator = monthDayCalculator;
    }

    public async Task<List<BillResponseDTO>> GetAllBills(string token)
    {
        var user = await _userAuthService.DecodeToken(token);
        return ConvertFromRepoDTOToDTO(await _dbService.GetAllBills(user));
    }

    public async Task AddBill(string token, NewBillRequestDTO newBill)
    {
        var user = await _userAuthService.DecodeToken(token);
        if (!await _accountDatabase.IsAccountOwnedByUser(user, newBill.AccountId))
        {
            throw new InvalidDataException("Account not found");
        }
        if (!_frequencyCalculation.DoesFrequencyExist(newBill.Frequency))
        {
            throw new InvalidDataException("Invalid frequency");
        }

        var dtoToDb = new NewBillEntity(
            _idGenerator.NewInt(await _dbService.GetLastId()),
            newBill.Payee,
            newBill.Amount,
            newBill.NextDueDate,
            newBill.Frequency,
            newBill.Category,
            _monthDayCalculator.Calculate(newBill.NextDueDate),
            newBill.AccountId
        );
        await _dbService.AddBill(dtoToDb);
    }

    public async Task EditBill(string token, EditBillRequestDTO editBill)
    {
        var user = await _userAuthService.DecodeToken(token);
        if (editBill.Payee == null && editBill.Amount == null &&
            editBill.Amount == null && editBill.NextDueDate == null &&
            editBill.Frequency == null && editBill.Category == null &&
            editBill.AccountId == null)
        {
            throw new InvalidDataException("Must have at least one value changed");
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

        var dtoToDb = new EditBillEntity(
            editBill.Id,
            editBill.Payee,
            editBill.Amount,
            editBill.NextDueDate,
            editBill.Frequency,
            editBill.Category,
            editBill.AccountId
        );
        await _dbService.EditBill(dtoToDb);
    }

    public async Task DeleteBill(string token, DeleteBillRequestDTO deleteBill)
    {
        var user = await _userAuthService.DecodeToken(token);
        if (!await _dbService.IsBillAssociatedWithUser(user, deleteBill.Id))
        {
            throw new InvalidDataException("Bill not found");
        }

        var dtoToDb = new DeleteBillDTO(
            deleteBill.Id
        );

        await _dbService.DeleteBill(user, dtoToDb);
    }

    public async Task SkipOccurence(string token, SkipBillOccurrenceRequestDTO skipBillDTO)
    {
        var user = await _userAuthService.DecodeToken(token);
        if (!await _dbService.IsBillAssociatedWithUser(user, skipBillDTO.Id))
        {
            throw new InvalidDataException("Bill not found");
        }

        var bill = await _dbService.GetBillById(user, skipBillDTO.Id);
        var newDueDate = _frequencyCalculation.CalculateNextDueDate(bill.Frequency, bill.MonthDay, skipBillDTO.SkipDatePastThisDate);

        var editBill = new EditBillEntity(skipBillDTO.Id, nextDueDate: newDueDate);
        await _dbService.EditBill(editBill);
    }

    private List<BillResponseDTO> ConvertFromRepoDTOToDTO(List<BillEntityDTO> billRepoDTO)
    {
        List<BillResponseDTO> res = [];
        foreach (var bill in billRepoDTO)
        {
            res.Add(new BillResponseDTO(
               bill.Id,
               bill.Payee,
               bill.Amount,
               bill.NextDueDate,
               bill.Frequency,
               bill.Category,
               _frequencyCalculation.CalculateOverDueBillInfo(bill.MonthDay, bill.Frequency,
                   bill.NextDueDate, _dateProvider),
               bill.AccountName
           ));
        }

        return res;
    }
}
