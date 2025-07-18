using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Authentication.Interfaces;
using MoneyTracker.Common.Result;
using MoneyTracker.Contracts.Responses.Budget;
using MoneyTracker.Queries.Domain.Entities.BudgetCategory;
using MoneyTracker.Queries.Domain.Handlers;
using MoneyTracker.Queries.Domain.Repositories.Service;

namespace MoneyTracker.Queries.Application;
public class BudgetService : IBudgetService
{
    private readonly IBudgetRepositoryService _budgetRepository;
    private readonly IUserRepositoryService _userRepository;

    public BudgetService(
        IBudgetRepositoryService budgetRepository,
        IUserRepositoryService userRepository)
    {
        _budgetRepository = budgetRepository;
        _userRepository = userRepository;
    }

    public async Task<ResultT<List<BudgetGroupResponse>>> GetBudget(string token, CancellationToken cancellationToken)
    {
        var userAuth = await _userRepository.GetUserAuthFromToken(token, cancellationToken);
        if (userAuth == null)
            throw new InvalidDataException("Token not found");
        userAuth.CheckValidation();

        var user = new AuthenticatedUser(userAuth.User.Id);
        var budgetResult = await _budgetRepository.GetBudget(user, cancellationToken);
        if (budgetResult.HasError)
            return budgetResult.Error!;

        return ConvertFromRepoDTOToDTO(budgetResult.Value);
    }

    private List<BudgetGroupResponse> ConvertFromRepoDTOToDTO(List<BudgetGroupEntity> billRepoDTO)
    {
        List<BudgetGroupResponse> res = [];
        foreach (var bill in billRepoDTO)
        {
            List<BudgetCategoryResponse> tmpCategoryLis = [];
            foreach (var category in bill.Categories)
            {
                tmpCategoryLis.Add(new(category.Id, category.Name, category.Planned, category.Actual, category.Difference));
            }

            res.Add(new BudgetGroupResponse(
                bill.Id,
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
