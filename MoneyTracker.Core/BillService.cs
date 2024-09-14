using MoneyTracker.Calculation.Bill;
using MoneyTracker.Shared.Core;
using MoneyTracker.Shared.Data;
using MoneyTracker.Shared.DateManager;
using MoneyTracker.Shared.Models.Bill;

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

    public async Task<List<BillDTO>> GetAllBills()
    {
        return ConvertFromRepoDTOToDTO(await _dbService.GetAllBills());
    }

    public async Task<List<BillDTO>> AddBill(NewBillDTO newBill)
    {
        return ConvertFromRepoDTOToDTO(await _dbService.AddBill(newBill));
    }

    public async Task<List<BillDTO>> EditBill(EditBillDTO editBill)
    {
        return ConvertFromRepoDTOToDTO(await _dbService.EditBill(editBill));
    }

    public async Task<List<BillDTO>> DeleteBill(DeleteBillDTO deleteBill)
    {
        return ConvertFromRepoDTOToDTO(await _dbService.DeleteBill(deleteBill));
    }

    public async Task<BillDTO> SkipOccurence(SkipBillOccurrenceDTO skipBillDTO)
    {
        var bill = await _dbService.GetBillById(skipBillDTO.Id);
        var newDueDate = BillCalculation.CalculateNextDueDate(bill.Frequency, bill.NextDueDate.Day, skipBillDTO.SkipDatePastThisDate);

        var editBill = new EditBillDTO(skipBillDTO.Id, nextDueDate: newDueDate);
        await _dbService.EditBill(editBill);

        return new BillDTO(
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

    private List<BillDTO> ConvertFromRepoDTOToDTO(List<BillFromRepositoryDTO> billRepoDTO)
    {
        List<BillDTO> res = [];
        foreach (var bill in billRepoDTO)
        {

            res.Add(new BillDTO(
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
