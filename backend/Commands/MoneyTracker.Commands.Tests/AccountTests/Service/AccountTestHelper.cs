
using MoneyTracker.Commands.Application;
using MoneyTracker.Commands.Domain.Handlers;
using MoneyTracker.Commands.Domain.Repositories;
using MoneyTracker.Common.Utilities.IdGeneratorUtil;
using MoneyTracker.PlatformService.Domain;
using Moq;

namespace MoneyTracker.Commands.Tests.AccountTests.Service;
public class AccountTestHelper
{
    public readonly Mock<IAccountCommandRepository> _mockAccountDatabase = new();
    public readonly Mock<IIdGenerator> _mockIdGenerator = new();
    public readonly Mock<IUserService> _mockUserService = new();
    public readonly Mock<IMessageBusClient> _mockMessageBusClient = new();

    public readonly AccountService _accountService;
    public AccountTestHelper()
    {
        _accountService = new AccountService(_mockAccountDatabase.Object,
            _mockIdGenerator.Object,
            _mockUserService.Object,
            _mockMessageBusClient.Object);
    }

    public void EnsureAllMocksHadNoOtherCalls()
    {
        _mockAccountDatabase.VerifyNoOtherCalls();
        _mockIdGenerator.VerifyNoOtherCalls();
        _mockUserService.VerifyNoOtherCalls();
        _mockMessageBusClient.VerifyNoOtherCalls();
    }
}
