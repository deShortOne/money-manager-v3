using MoneyTracker.Authentication.Interfaces;
using MoneyTracker.Common.Utilities.CalculationUtil;
using MoneyTracker.Common.Utilities.DateTimeUtil;
using MoneyTracker.Contracts.Responses.Bill;
using MoneyTracker.Queries.Domain.Entities.Bill;
using MoneyTracker.Queries.Domain.Handlers;
using MoneyTracker.Queries.Domain.Repositories;

namespace MoneyTracker.Queries.Application;
public class BillService : IBillService
{
    private readonly IBillRepository _dbService;
    private readonly IUserAuthenticationService _userAuthService;
    private readonly IFrequencyCalculation _frequencyCalculation;

    public BillService(IBillRepository dbService,
        IUserAuthenticationService userAuthService,
        IFrequencyCalculation frequencyCalculation)
    {
        _dbService = dbService;
        _userAuthService = userAuthService;
        _frequencyCalculation = frequencyCalculation;
    }

    public async Task<List<BillResponse>> GetAllBills(string token)
    {
        var user = await _userAuthService.DecodeToken(token);
        return ConvertFromRepoDTOToDTO(await _dbService.GetAllBills(user));
    }

    private List<BillResponse> ConvertFromRepoDTOToDTO(List<BillEntity> billRepoDTO)
    {
        List<BillResponse> res = [];
        foreach (var bill in billRepoDTO)
        {
            res.Add(new BillResponse(
               bill.Id,
               bill.Payee,
               bill.Amount,
               bill.NextDueDate,
               bill.Frequency,
               bill.CategoryName,
               _frequencyCalculation.CalculateOverDueBillInfo(bill.MonthDay, bill.Frequency,
                   bill.NextDueDate),
               bill.AccountName
           ));
        }

        return res;
    }
}
