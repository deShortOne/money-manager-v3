
using MoneyTracker.Data.Postgres;
using MoneyTracker.DatabaseMigration;
using MoneyTracker.DatabaseMigration.Models;
using MoneyTracker.Shared.Auth;
using MoneyTracker.Shared.Models.RepositoryToService.Bill;
using MoneyTracker.Shared.Models.ServiceToRepository.Bill;
using Testcontainers.PostgreSql;

namespace MoneyTracker.Tests.Database.Postgres;
public class BillTest : IAsyncLifetime
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
    public async void FirstLoadCheckTablesThatDataAreThere()
    {
        var db = new PostgresDatabase(_postgres.GetConnectionString());
        var bill = new BillDatabase(db);

        var expected = new List<BillEntityDTO>()
        {
            new(2, "company a", 100, new DateOnly(2024, 08, 30), "Monthly", "Wages & Salary : Net Pay", 30, "bank b"),
            new(1, "supermarket a", 23, new DateOnly(2024, 09, 03), "Weekly", "Groceries", 3, "bank a"),
        };

        var actual = await bill.GetAllBills(new AuthenticatedUser(1));

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async void FirstLoadCheckTablesThatDataAreThereUser2()
    {
        var db = new PostgresDatabase(_postgres.GetConnectionString());
        var bill = new BillDatabase(db);

        var expected = new List<BillEntityDTO>()
        {
            new(3, "company a", 100, new DateOnly(2024, 08, 30), "Monthly", "Wages & Salary : Net Pay", 30, "bank a"),
        };

        var actual = await bill.GetAllBills(new AuthenticatedUser(2));

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async void DeleteBill()
    {
        var user = new AuthenticatedUser(1);
        var db = new PostgresDatabase(_postgres.GetConnectionString());
        var bill = new BillDatabase(db);
        await bill.DeleteBill(1);

        var expected = new List<BillEntityDTO>()
        {
            new(2, "company a", 100, new DateOnly(2024, 08, 30), "Monthly", "Wages & Salary : Net Pay", 30, "bank b"),
        };

        var actual = await bill.GetAllBills(user);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async void EditBill()
    {
        var user = new AuthenticatedUser(1);
        var db = new PostgresDatabase(_postgres.GetConnectionString());
        var bill = new BillDatabase(db);
        await bill.EditBill(new EditBillEntity(1, payee: "supermarket b"));

        var expected = new List<BillEntityDTO>()
        {
            new(2, "company a", 100, new DateOnly(2024, 08, 30), "Monthly", "Wages & Salary : Net Pay", 30, "bank b"),
            new(1, "supermarket b", 23, new DateOnly(2024, 09, 03), "Weekly", "Groceries", 3,   "bank a"),
        };

        var actual = await bill.GetAllBills(user);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async void EditBill_UpdateNextDueDate_MonthDayAlsoUpdates()
    {
        var user = new AuthenticatedUser(1);
        var db = new PostgresDatabase(_postgres.GetConnectionString());
        var bill = new BillDatabase(db);
        await bill.EditBill(new EditBillEntity(1, nextDueDate: new DateOnly(2024, 5, 5)));
        await bill.EditBill(new EditBillEntity(2, nextDueDate: new DateOnly(2024, 10, 17)));

        var expected = new List<BillEntityDTO>()
        {
            new(1, "supermarket a", 23, new DateOnly(2024, 05, 05), "Weekly", "Groceries", 5, "bank a"),
            new(2, "company a", 100, new DateOnly(2024, 10, 17), "Monthly", "Wages & Salary : Net Pay", 17, "bank b"),
        };

        var actual = await bill.GetAllBills(user);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async void AddBill()
    {
        var user = new AuthenticatedUser(1);
        var db = new PostgresDatabase(_postgres.GetConnectionString());
        var bill = new BillDatabase(db);
        await bill.AddBill(new BillEntity(4, "flight sim", 420, new DateOnly(2024, 09, 05), "Daily", 5, 5, 1));

        var expected = new List<BillEntityDTO>()
        {
            new(2, "company a", 100, new DateOnly(2024, 08, 30), "Monthly", "Wages & Salary : Net Pay", 30, "bank b"),
            new(1, "supermarket a", 23, new DateOnly(2024, 09, 03), "Weekly", "Groceries", 3, "bank a"),
            new(4, "flight sim", 420, new DateOnly(2024, 09, 05), "Daily", "Hobby", 5, "bank a"),
        };

        var actual = await bill.GetAllBills(user);

        Assert.Equal(expected, actual);
    }
}
