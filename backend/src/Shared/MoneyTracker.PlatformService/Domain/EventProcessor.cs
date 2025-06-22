using MoneyTracker.PlatformService.DTOs;
using MoneyTracker.Queries.Domain.Repositories.Service;

namespace MoneyTracker.PlatformService.Domain;
public class EventProcessor : IEventProcessor
{
    private readonly IAccountRepositoryService _accountRepositoryService;
    private readonly IBillRepositoryService _billRepositoryService;
    private readonly IBudgetRepositoryService _budgetRepositoryService;
    private readonly ICategoryRepositoryService _categoryRepositoryService;
    private readonly IRegisterRepositoryService _registerRepositoryService;
    private readonly IUserRepositoryService _userRepositoryService;

    public EventProcessor(
        IAccountRepositoryService accountRepositoryService,
        IBillRepositoryService billRepositoryService,
        IBudgetRepositoryService budgetRepositoryService,
        ICategoryRepositoryService categoryRepositoryService,
        IRegisterRepositoryService registerRepositoryService,
        IUserRepositoryService userRepositoryService
        )
    {
        _accountRepositoryService = accountRepositoryService;
        _billRepositoryService = billRepositoryService;
        _budgetRepositoryService = budgetRepositoryService;
        _categoryRepositoryService = categoryRepositoryService;
        _registerRepositoryService = registerRepositoryService;
        _userRepositoryService = userRepositoryService;
    }

    public void ProcessEvent(EventUpdate eventUpdate)
    {
        switch (eventUpdate.Name)
        {
            case DataTypes.Account:
                _accountRepositoryService.ResetAccountsCache(eventUpdate.User);
                break;
            case DataTypes.Bill:
                _billRepositoryService.ResetBillsCache(eventUpdate.User);
                break;
            case DataTypes.Budget:
                _budgetRepositoryService.ResetBudgetCache(eventUpdate.User);
                break;
            case DataTypes.Category:
                _categoryRepositoryService.ResetCategoriesCache();
                break;
            case DataTypes.Register:
                _registerRepositoryService.ResetTransactionsCache(eventUpdate.User);
                break;
            case DataTypes.User:
                _userRepositoryService.ResetUsersCache();
                break;
            default:
                // log
                break;
        }
    }
}
