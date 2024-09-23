
using MoneyTracker.Core;
using MoneyTracker.Data.Postgres;
using MoneyTracker.DatabaseMigration;
using MoneyTracker.DatabaseMigration.Models;
using MoneyTracker.Shared.Auth;
using MoneyTracker.Shared.Core;
using MoneyTracker.Shared.Models.ControllerToService.Transaction;
using MoneyTracker.Shared.Models.ServiceToController.Transaction;
using Moq;
using Testcontainers.PostgreSql;

namespace MoneyTracker.Tests.Core;
public sealed class RegisterServiceTest : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
#if RUN_LOCAL
       .WithDockerEndpoint("tcp://localhost:2375")
#endif
       .WithImage("postgres:16")
       .WithCleanUp(true)
       .Build();

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();

        Migration.CheckMigration(_postgres.GetConnectionString(), new MigrationOption(true));

        return;
    }

    public Task DisposeAsync()
    {
        return _postgres.DisposeAsync().AsTask();
    }

    [Fact]
    public async void EditTransaction_ChangeCategory_ReturnsTransactionWithNewCategory()
    {
        var db = new PostgresDatabase(_postgres.GetConnectionString());
        var registerDb = new RegisterDatabase(db);
        var accountDb = new AccountDatabase(db);

        var mockAuthService = new Mock<IUserAuthenticationService>();
        mockAuthService.Setup(x => x.DecodeToken(It.IsAny<string>())).Returns(Task.FromResult(new AuthenticatedUser(1)));

        var registerService = new RegisterService(registerDb, mockAuthService.Object, accountDb);
        var returnedTransaction = await registerService.EditTransaction("", new EditTransactionRequestDTO(2, category: 5));
        Assert.Equal(new TransactionResponseDTO(2, "Phone company", 10, new DateOnly(2024, 8, 1), "Hobby", "bank a"), returnedTransaction);
    }
}
