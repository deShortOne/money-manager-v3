using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Common.Result;
using MoneyTracker.Common.Utilities.CalculationUtil;
using MoneyTracker.Contracts.Responses.Account;
using MoneyTracker.Contracts.Responses.Bill;
using MoneyTracker.Contracts.Responses.Category;
using MoneyTracker.Queries.Domain.Entities.Bill;
using MoneyTracker.Queries.Domain.Handlers;
using MoneyTracker.Queries.Domain.Repositories.Database;

namespace MoneyTracker.Queries.Application;
public class BillService : IBillService
{
    private readonly IBillDatabase _dbService;
    private readonly IFrequencyCalculation _frequencyCalculation;
    private readonly IUserDatabase _userRepository;

    public BillService(IBillDatabase dbService,
        IFrequencyCalculation frequencyCalculation,
        IUserDatabase userRepository)
    {
        _dbService = dbService;
        _frequencyCalculation = frequencyCalculation;
        _userRepository = userRepository;
    }

    public async Task<ResultT<List<BillResponse>>> GetAllBills(string token)
    {
        var userAuth = await _userRepository.GetUserAuthFromToken(token);
        if (userAuth == null)
            throw new InvalidDataException("Token not found");
        userAuth.CheckValidation();

        var user = new AuthenticatedUser(userAuth.User.Id);
        var billsResult = await _dbService.GetAllBills(user);
        if (!billsResult.IsSuccess)
            return billsResult.Error!;

        return ConvertFromRepoDTOToDTO(billsResult.Value);
    }

    public Task<List<string>> GetAllFrequencyNames()
    {
        return Task.FromResult(_frequencyCalculation.GetFrequencyNames());
    }

    private List<BillResponse> ConvertFromRepoDTOToDTO(List<BillEntity> billRepoDTO)
    {
        List<BillResponse> res = [];
        foreach (var bill in billRepoDTO)
        {
            res.Add(new BillResponse(
                bill.Id,
                new(
                    bill.PayeeId,
                    bill.PayeeName
                ),
                bill.Amount,
                bill.NextDueDate,
                bill.Frequency,
                new(
                    bill.CategoryId,
                    bill.CategoryName
                ),
                _frequencyCalculation.CalculateOverDueBillInfo(
                    bill.MonthDay,
                    bill.Frequency,
                   bill.NextDueDate
                ),
                new(
                    bill.PayerId,
                    bill.PayerName
                )
           ));
        }

        return res;
    }
}
