using MoneyTracker.Queries.Application;
using MoneyTracker.Queries.Domain.Repositories.Service;
using Moq;

namespace MoneyTracker.Queries.Tests.RegisterTests.Service;
public class RegisterTestHelper
{
    public readonly Mock<IRegisterRepositoryService> _mockRegisterDatabase = new();
    public readonly Mock<IUserRepositoryService> _mockUserRepository = new();

    public readonly RegisterService _budgetService;

    public RegisterTestHelper()
    {
        _budgetService = new RegisterService(_mockRegisterDatabase.Object, _mockUserRepository.Object);
    }

    public void EnsureAllMocksHadNoOtherCalls()
    {
        _mockRegisterDatabase.VerifyNoOtherCalls();
        _mockUserRepository.VerifyNoOtherCalls();
    }
}
