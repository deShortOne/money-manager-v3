
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
    public async void AddTransaction_AttemptToYseAccountNotOwnedByUser_Errors()
    {
        var db = new PostgresDatabase(_postgres.GetConnectionString());
        var registerDb = new RegisterDatabase(db);
        var accountDb = new AccountDatabase(db);

        var mockAuthService = new Mock<IUserAuthenticationService>();
        mockAuthService.Setup(x => x.DecodeToken(It.IsAny<string>())).Returns(Task.FromResult(new AuthenticatedUser(1)));

        var registerService = new RegisterService(registerDb, mockAuthService.Object, accountDb);

        var error = await Assert.ThrowsAsync<InvalidDataException>(async () =>
        {
            await registerService.AddTransaction("", new NewTransactionRequestDTO("Super star", 2300, new DateOnly(2024, 9, 1), 5, 8));
        });
        Assert.Equal("Account not found", error.Message);
    }

    [Fact]
    public async void EditTransaction_ChangeCategory_Succeeds()
    {
        var db = new PostgresDatabase(_postgres.GetConnectionString());
        var registerDb = new RegisterDatabase(db);
        var accountDb = new AccountDatabase(db);

        var mockAuthService = new Mock<IUserAuthenticationService>();
        mockAuthService.Setup(x => x.DecodeToken(It.IsAny<string>())).Returns(Task.FromResult(new AuthenticatedUser(1)));

        var registerService = new RegisterService(registerDb, mockAuthService.Object, accountDb);
        await registerService.EditTransaction("", new EditTransactionRequestDTO(2, category: 5));

        var actual = await registerService.GetAllTransactions("");

        var expected = new List<TransactionResponseDTO>()
        {
            new(1, "Company A", 1800, new DateOnly(2024, 8, 28), "Wages & Salary : Net Pay", "bank a"),
            new(6, "Supermarket", 27, new DateOnly(2024, 8, 15), "Groceries", "bank b"),
            new(7, "Hobby item", 150, new DateOnly(2024, 8, 9), "Hobby", "bank a"),
            new(5, "Supermarket", 23, new DateOnly(2024, 8, 8), "Groceries", "bank b"),
            new(3, "Landlord A", 500, new DateOnly(2024, 8, 1), "Bills : Rent", "bank a"),
            new(4, "Supermarket", 25, new DateOnly(2024, 8, 1), "Groceries", "bank b"),
            new(2, "Phone company", 10, new DateOnly(2024, 8, 1), "Hobby", "bank a"),
        };

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async void EditTransaction_ChangeTransactionToAccountNotOwnedByUser_Errors()
    {
        var db = new PostgresDatabase(_postgres.GetConnectionString());
        var registerDb = new RegisterDatabase(db);
        var accountDb = new AccountDatabase(db);

        var mockAuthService = new Mock<IUserAuthenticationService>();
        mockAuthService.Setup(x => x.DecodeToken(It.IsAny<string>())).Returns(Task.FromResult(new AuthenticatedUser(1)));

        var registerService = new RegisterService(registerDb, mockAuthService.Object, accountDb);

        var error = await Assert.ThrowsAsync<InvalidDataException>(async () =>
        {
            await registerService.EditTransaction("", new EditTransactionRequestDTO(2, accountId: 5));
        });
        Assert.Equal("Account not found", error.Message);
    }

    [Fact]
    public async void EditTransaction_ChangeTransactionNotOwnedByUser_Errors()
    {
        var db = new PostgresDatabase(_postgres.GetConnectionString());
        var registerDb = new RegisterDatabase(db);
        var accountDb = new AccountDatabase(db);

        var mockAuthService = new Mock<IUserAuthenticationService>();
        mockAuthService.Setup(x => x.DecodeToken(It.IsAny<string>())).Returns(Task.FromResult(new AuthenticatedUser(1)));

        var registerService = new RegisterService(registerDb, mockAuthService.Object, accountDb);

        var error = await Assert.ThrowsAsync<InvalidDataException>(async () =>
        {
            await registerService.EditTransaction("", new EditTransactionRequestDTO(8, accountId: 5));
        });
        Assert.Equal("Transaction not found", error.Message);
    }

    [Fact]
    public async void DeleteTransaction_ChangeTransactionNotOwnedByUser_Errors()
    {
        var db = new PostgresDatabase(_postgres.GetConnectionString());
        var registerDb = new RegisterDatabase(db);
        var accountDb = new AccountDatabase(db);

        var mockAuthService = new Mock<IUserAuthenticationService>();
        mockAuthService.Setup(x => x.DecodeToken(It.IsAny<string>())).Returns(Task.FromResult(new AuthenticatedUser(1)));

        var registerService = new RegisterService(registerDb, mockAuthService.Object, accountDb);

        var error = await Assert.ThrowsAsync<InvalidDataException>(async () =>
        {
            await registerService.DeleteTransaction("", new DeleteTransactionRequestDTO(8));
        });
        Assert.Equal("Transaction not found", error.Message);
    }
}
