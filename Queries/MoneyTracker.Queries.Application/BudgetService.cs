using MoneyTracker.Authentication.Authentication;
using MoneyTracker.Contracts.Responses.Budget;
using MoneyTracker.Queries.Domain.Entities.BudgetCategory;
using MoneyTracker.Queries.Domain.Handlers;
using MoneyTracker.Queries.Domain.Repositories;

namespace MoneyTracker.Queries.Application;
public class BudgetService : IBudgetService
{
    private readonly UserAuthenticationService _userAuthService;
    private readonly IBudgetRepository _dbService;

    public BudgetService(UserAuthenticationService userAuthService,
        IBudgetRepository dbService)
    {
        _userAuthService = userAuthService;
        _dbService = dbService;
    }

    public async Task<List<BudgetGroupResponse>> GetBudget(string token)
    {
        var user = await _userAuthService.DecodeToken(token);
        return ConvertFromRepoDTOToDTO(await _dbService.GetBudget(user));
    }

    private List<BudgetGroupResponse> ConvertFromRepoDTOToDTO(List<BudgetGroupEntity> billRepoDTO)
    {
        List<BudgetGroupResponse> res = [];
        foreach (var bill in billRepoDTO)
        {
            List<BudgetCategoryResponse> tmpCategoryLis = [];
            foreach (var category in bill.Categories)
            {
                tmpCategoryLis.Add(new(category.Name, category.Planned, category.Actual, category.Difference));
            }

            res.Add(new BudgetGroupResponse(
                bill.Name,
                bill.Planned,
                bill.Actual,
                bill.Difference,
                tmpCategoryLis
           ));
        }

        return res;
    }
}
