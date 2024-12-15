using MoneyTracker.Authentication.Interfaces;
using MoneyTracker.Common.Utilities.CalculationUtil;
using MoneyTracker.Queries.Application;
using MoneyTracker.Queries.Domain.Repositories;
using Moq;

namespace MoneyTracker.Queries.Tests.AccountTests.Service;
public class AccountTestHelper
{
    public readonly Mock<IAccountRepository> _mockAccountDatabase = new();
    public readonly Mock<IUserAuthenticationService> _mockUserAuthService = new();

    public readonly AccountService _accountService;

    public AccountTestHelper()
    {
        _accountService = new AccountService(_mockUserAuthService.Object,
            _mockAccountDatabase.Object);
    }

    public void EnsureAllMocksHadNoOtherCalls()
    {
        _mockAccountDatabase.VerifyNoOtherCalls();
        _mockUserAuthService.VerifyNoOtherCalls();
    }
}
