using MoneyTracker.Authentication.Interfaces;
using MoneyTracker.Common.Utilities.CalculationUtil;
using MoneyTracker.Queries.Application;
using MoneyTracker.Queries.Domain.Repositories.Database;
using Moq;

namespace MoneyTracker.Queries.Tests.AccountTests.Service;
public class AccountTestHelper
{
    public readonly Mock<IAccountDatabase> _mockAccountDatabase = new();
    public readonly Mock<IUserDatabase> _mockUserRepository = new();

    public readonly AccountService _accountService;

    public AccountTestHelper()
    {
        _accountService = new AccountService(
            _mockAccountDatabase.Object,
            _mockUserRepository.Object);
    }

    public void EnsureAllMocksHadNoOtherCalls()
    {
        _mockAccountDatabase.VerifyNoOtherCalls();
        _mockUserRepository.VerifyNoOtherCalls();
    }
}
