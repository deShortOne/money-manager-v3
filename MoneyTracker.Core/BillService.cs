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

    public BillService(IBillDatabase dbService, IDateProvider dateProvider)
    {
        _dbService = dbService;
        _dateProvider = dateProvider;
    }

    public async Task<List<BillResponseDTO>> GetAllBills()
    {
        return ConvertFromRepoDTOToDTO(await _dbService.GetAllBills());
    }

    public async Task<List<BillResponseDTO>> AddBill(NewBillRequestDTO newBill)
    {
        var dtoToDb = new NewBillDTO(
            newBill.Payee,
            newBill.Amount,
            newBill.NextDueDate,
            newBill.Frequency,
            newBill.Category,
            newBill.MonthDay
        );
        return ConvertFromRepoDTOToDTO(await _dbService.AddBill(dtoToDb));
    }

    public async Task<List<BillResponseDTO>> EditBill(EditBillRequestDTO editBill)
    {
        var dtoToDb = new EditBillDTO(
            editBill.Id,
            editBill.Payee,
            editBill.Amount,
            editBill.NextDueDate,
            editBill.Frequency,
            editBill.Category
        );
        return ConvertFromRepoDTOToDTO(await _dbService.EditBill(dtoToDb));
    }

    public async Task<List<BillResponseDTO>> DeleteBill(DeleteBillRequestDTO deleteBill)
    {
        var dtoToDb = new DeleteBillDTO(
            deleteBill.Id
        );
        return ConvertFromRepoDTOToDTO(await _dbService.DeleteBill(dtoToDb));
    }

    public async Task<BillResponseDTO> SkipOccurence(SkipBillOccurrenceRequestDTO skipBillDTO)
    {
        var bill = await _dbService.GetBillById(skipBillDTO.Id);
        var newDueDate = BillCalculation.CalculateNextDueDate(bill.Frequency, bill.MonthDay, skipBillDTO.SkipDatePastThisDate);

        var editBill = new EditBillDTO(skipBillDTO.Id, nextDueDate: newDueDate);
        await _dbService.EditBill(editBill);

        return new BillResponseDTO(
               bill.Id,
               bill.Payee,
               bill.Amount,
               newDueDate,
               bill.Frequency,
               bill.Category,
               BillCalculation.CalculateOverDueBillInfo(bill.MonthDay, bill.Frequency,
                   newDueDate, _dateProvider)
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
                   bill.NextDueDate, _dateProvider)
           ));
        }

        return res;
    }
}
