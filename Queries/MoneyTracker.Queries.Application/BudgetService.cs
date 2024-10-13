using MoneyTracker.Contracts.Responses.Budget;
using MoneyTracker.Queries.Domain.Entities.BudgetCategory;
using MoneyTracker.Queries.Domain.Handlers;
using MoneyTracker.Queries.Domain.Repositories;

namespace MoneyTracker.Queries.Application;
public class BudgetService : IBudgetService
{
    private readonly IBudgetRepository _dbService;

    public BudgetService(IBudgetRepository dbService)
    {
        _dbService = dbService;
    }

    public async Task<List<BudgetGroupResponse>> GetBudget()
    {
        return ConvertFromRepoDTOToDTO(await _dbService.GetBudget());
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
