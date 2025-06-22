using MoneyTracker.Commands.Application;
using MoneyTracker.Commands.Domain.Handlers;
using MoneyTracker.Commands.Domain.Repositories;
using MoneyTracker.PlatformService.Domain;
using Moq;

namespace MoneyTracker.Commands.Tests.BudgetTests.Service;
public class BudgetTestHelper
{
    public readonly Mock<IBudgetCommandRepository> _mockBudgetCategoryDatabase = new();
    public readonly Mock<IUserCommandRepository> _mockUserRepository = new();
    public readonly Mock<IUserService> _mockUserService = new();
    public readonly Mock<IMessageBusClient> _mockMessageBusClient = new();

    public readonly BudgetService _budgetService;

    public BudgetTestHelper()
    {
        _budgetService = new BudgetService(
            _mockBudgetCategoryDatabase.Object,
            _mockUserRepository.Object,
            _mockUserService.Object,
            _mockMessageBusClient.Object
        );
    }

    public void EnsureAllMocksHadNoOtherCalls()
    {
        _mockBudgetCategoryDatabase.VerifyNoOtherCalls();
        _mockUserRepository.VerifyNoOtherCalls();
        _mockUserService.VerifyNoOtherCalls();
        _mockMessageBusClient.VerifyNoOtherCalls();
    }
}
