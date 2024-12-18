using MoneyTracker.Authentication.Interfaces;
using MoneyTracker.Queries.Application;
using MoneyTracker.Queries.Domain.Repositories;
using Moq;

namespace MoneyTracker.Queries.Tests.RegisterTests.Service;
public class RegisterTestHelper
{
    public readonly Mock<IRegisterRepository> _mockRegisterDatabase = new();
    public readonly Mock<IUserAuthenticationService> _mockUserAuthService = new();

    public readonly RegisterService _budgetService;

    public RegisterTestHelper()
    {
        _budgetService = new RegisterService(_mockRegisterDatabase.Object, _mockUserAuthService.Object);
    }

    public void EnsureAllMocksHadNoOtherCalls()
    {
        _mockRegisterDatabase.VerifyNoOtherCalls();
        _mockUserAuthService.VerifyNoOtherCalls();
    }
}
