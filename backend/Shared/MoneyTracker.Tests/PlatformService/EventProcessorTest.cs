
using MoneyTracker.Authentication.DTOs;
using MoneyTracker.PlatformService.Domain;
using MoneyTracker.PlatformService.DTOs;
using MoneyTracker.Queries.Domain.Repositories.Service;
using Moq;

namespace MoneyTracker.Tests.PlatformService;
public sealed class EventProcessorTest
{
    AuthenticatedUser _authenticatedUser = new AuthenticatedUser(42);

    private Mock<IAccountRepositoryService> _accountRepositoryService;
    private Mock<IBillRepositoryService> _billRepositoryService;
    private Mock<IBudgetRepositoryService> _budgetRepositoryService;
    private Mock<ICategoryRepositoryService> _cateogryRepositoryService;
    private Mock<IRegisterRepositoryService> _registerRepositoryService;
    private Mock<IUserRepositoryService> _userRepositoryService;
    private EventProcessor _eventProcessor;

    public EventProcessorTest()
    {
        _accountRepositoryService = new Mock<IAccountRepositoryService>();
        _billRepositoryService = new Mock<IBillRepositoryService>();
        _budgetRepositoryService = new Mock<IBudgetRepositoryService>();
        _cateogryRepositoryService = new Mock<ICategoryRepositoryService>();
        _registerRepositoryService = new Mock<IRegisterRepositoryService>();
        _userRepositoryService = new Mock<IUserRepositoryService>();

        _eventProcessor = new EventProcessor(
            _accountRepositoryService.Object,
            _billRepositoryService.Object,
            _budgetRepositoryService.Object,
            _cateogryRepositoryService.Object,
            _registerRepositoryService.Object,
            _userRepositoryService.Object
        );
    }

    private void NoOtherCalls()
    {
        _accountRepositoryService.VerifyNoOtherCalls();
        _billRepositoryService.VerifyNoOtherCalls();
        _budgetRepositoryService.VerifyNoOtherCalls();
        _cateogryRepositoryService.VerifyNoOtherCalls();
        _registerRepositoryService.VerifyNoOtherCalls();
        _userRepositoryService.VerifyNoOtherCalls();
    }

    [Fact]
    public void SuccessfullyUpdateAccountCache()
    {
        _eventProcessor.ProcessEvent(new EventUpdate(_authenticatedUser, DataTypes.Account));

        _accountRepositoryService.Verify(x => x.ResetAccountsCache(_authenticatedUser));
        NoOtherCalls();
    }

    [Fact]
    public void SuccessfullyUpdateBillCache()
    {
        _eventProcessor.ProcessEvent(new EventUpdate(_authenticatedUser, DataTypes.Bill));

        _billRepositoryService.Verify(x => x.ResetBillsCache(_authenticatedUser));
        NoOtherCalls();
    }

    [Fact]
    public void SuccessfullyUpdateBudgetCache()
    {
        _eventProcessor.ProcessEvent(new EventUpdate(_authenticatedUser, DataTypes.Budget));

        _budgetRepositoryService.Verify(x => x.ResetBudgetCache(_authenticatedUser));
        NoOtherCalls();
    }

    [Fact]
    public void SuccessfullyUpdateCategoryCache()
    {
        _eventProcessor.ProcessEvent(new EventUpdate(_authenticatedUser, DataTypes.Category));

        _cateogryRepositoryService.Verify(x => x.ResetCategoriesCache());
        NoOtherCalls();
    }

    [Fact]
    public void SuccessfullyUpdateRegisterCache()
    {
        _eventProcessor.ProcessEvent(new EventUpdate(_authenticatedUser, DataTypes.Register));

        _registerRepositoryService.Verify(x => x.ResetTransactionsCache(_authenticatedUser));
        NoOtherCalls();
    }

    [Fact]
    public void SuccessfullyUpdateUserCache()
    {
        _eventProcessor.ProcessEvent(new EventUpdate(_authenticatedUser, DataTypes.User));

        _userRepositoryService.Verify(x => x.ResetUsersCache());
        NoOtherCalls();
    }

    [Fact]
    public void FailToUpdateCacheDueToUnknownDataType()
    {
        _eventProcessor.ProcessEvent(new EventUpdate(_authenticatedUser, ""));

        NoOtherCalls();
    }
}
