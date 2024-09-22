using MoneyTracker.Calculation.Bill;
using MoneyTracker.Shared.Core;
using MoneyTracker.Shared.Data;
using MoneyTracker.Shared.DateManager;
using MoneyTracker.Shared.Models.ControllerToService.Bill;
using MoneyTracker.Shared.Models.RepositoryToService.Bill;
using MoneyTracker.Shared.Models.ServiceToController.Bill;
using MoneyTracker.Shared.Models.ServiceToRepository.Bill;

namespace MoneyTracker.Core;
public class BillService : IBillService
{
    private readonly IBillDatabase _dbService;
    private readonly IDateProvider _dateProvider;
    private readonly IUserAuthenticationService _userAuthService;

    public BillService(IBillDatabase dbService, IDateProvider dateProvider,
        IUserAuthenticationService userAuthService)
    {
        _dbService = dbService;
        _dateProvider = dateProvider;
        _userAuthService = userAuthService;
    }

    public async Task<List<BillResponseDTO>> GetAllBills(string token)
    {
        var user = await _userAuthService.DecodeToken(token);
        return ConvertFromRepoDTOToDTO(await _dbService.GetAllBills(user));
    }

    public async Task<List<BillResponseDTO>> AddBill(string token, NewBillRequestDTO newBill)
    {
        var user = await _userAuthService.DecodeToken(token);
        var dtoToDb = new NewBillDTO(
            newBill.Payee,
            newBill.Amount,
            newBill.NextDueDate,
            newBill.Frequency,
            newBill.Category,
            newBill.MonthDay,
            newBill.AccountId
        );
        return ConvertFromRepoDTOToDTO(await _dbService.AddBill(user, dtoToDb));
    }

    public async Task<List<BillResponseDTO>> EditBill(string token, EditBillRequestDTO editBill)
    {
        var user = await _userAuthService.DecodeToken(token);
        if (!await _dbService.IsBillAssociatedWithUser(user, editBill.Id))
        {
            throw new InvalidDataException("Bill id not found");
        }

        var dtoToDb = new EditBillDTO(
            editBill.Id,
            editBill.Payee,
            editBill.Amount,
            editBill.NextDueDate,
            editBill.Frequency,
            editBill.Category,
            editBill.AccountId
        );
        return ConvertFromRepoDTOToDTO(await _dbService.EditBill(user, dtoToDb));
    }

    public async Task<List<BillResponseDTO>> DeleteBill(string token, DeleteBillRequestDTO deleteBill)
    {
        var user = await _userAuthService.DecodeToken(token);
        if (!await _dbService.IsBillAssociatedWithUser(user, deleteBill.Id))
        {
            throw new InvalidDataException("Bill id not found");
        }

        var dtoToDb = new DeleteBillDTO(
            deleteBill.Id
        );
        return ConvertFromRepoDTOToDTO(await _dbService.DeleteBill(user, dtoToDb));
    }

    public async Task<BillResponseDTO> SkipOccurence(string token, SkipBillOccurrenceRequestDTO skipBillDTO)
    {
        var user = await _userAuthService.DecodeToken(token);
        if (!await _dbService.IsBillAssociatedWithUser(user, skipBillDTO.Id))
        {
            throw new InvalidDataException("Bill id not found");
        }

        var bill = await _dbService.GetBillById(user, skipBillDTO.Id);
        var newDueDate = BillCalculation.CalculateNextDueDate(bill.Frequency, bill.MonthDay, skipBillDTO.SkipDatePastThisDate);

        var editBill = new EditBillDTO(skipBillDTO.Id, nextDueDate: newDueDate);
        await _dbService.EditBill(user, editBill);

        return new BillResponseDTO(
               bill.Id,
               bill.Payee,
               bill.Amount,
               newDueDate,
               bill.Frequency,
               bill.Category,
               BillCalculation.CalculateOverDueBillInfo(bill.MonthDay, bill.Frequency,
                   newDueDate, _dateProvider),
               bill.AccountName
           );
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
               BillCalculation.CalculateOverDueBillInfo(bill.MonthDay, bill.Frequency,
                   bill.NextDueDate, _dateProvider),
               bill.AccountName
           ));
        }

        return res;
    }
}
