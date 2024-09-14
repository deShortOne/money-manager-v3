
using MoneyTracker.Data.Postgres;
using MoneyTracker.DatabaseMigration;
using MoneyTracker.DatabaseMigration.Models;
using MoneyTracker.Shared.DateManager;
using MoneyTracker.Shared.Models.Bill;
using MoneyTracker.Tests.Local;
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

        var expected = new List<BillFromRepositoryDTO>()
        {
            new(2, "company a", 100, DateOnly.Parse("2024-08-30"), "Monthly", "Wages & Salary : Net Pay", 30),
            new(1, "supermarket a", 23, DateOnly.Parse("2024-09-03"), "Weekly", "Groceries", 3),
        };

        var actual = await bill.GetAllBills();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async void DeleteBill()
    {
        var db = new PostgresDatabase(_postgres.GetConnectionString());
        var bill = new BillDatabase(db);
        await bill.DeleteBill(new DeleteBillDTO(1));

        var expected = new List<BillFromRepositoryDTO>()
        {
            new(2, "company a", 100, DateOnly.Parse("2024-08-30"), "Monthly", "Wages & Salary : Net Pay", 30),
        };

        var actual = await bill.GetAllBills();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async void EditBill()
    {
        var db = new PostgresDatabase(_postgres.GetConnectionString());
        var bill = new BillDatabase(db);
        await bill.EditBill(new EditBillDTO(1, payee: "supermarket b"));

        var expected = new List<BillFromRepositoryDTO>()
        {
            new(2, "company a", 100, DateOnly.Parse("2024-08-30"), "Monthly", "Wages & Salary : Net Pay", 30),
            new(1, "supermarket b", 23, DateOnly.Parse("2024-09-03"), "Weekly", "Groceries", 3),
        };

        var actual = await bill.GetAllBills();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async void EditBill_UpdateNextDueDate_MonthDayAlsoUpdates()
    {
        var db = new PostgresDatabase(_postgres.GetConnectionString());
        var bill = new BillDatabase(db);
        await bill.EditBill(new EditBillDTO(1, nextDueDate: new DateOnly(2024, 5, 5)));
        await bill.EditBill(new EditBillDTO(2, nextDueDate: new DateOnly(2024, 10, 17)));

        var expected = new List<BillFromRepositoryDTO>()
        {
            new(1, "supermarket a", 23, DateOnly.Parse("2024-05-05"), "Weekly", "Groceries", 5),
            new(2, "company a", 100, DateOnly.Parse("2024-10-17"), "Monthly", "Wages & Salary : Net Pay", 17),
        };

        var actual = await bill.GetAllBills();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async void AddBill()
    {
        var db = new PostgresDatabase(_postgres.GetConnectionString());
        var bill = new BillDatabase(db);
        await bill.AddBill(new NewBillDTO("flight sim", 420, DateOnly.Parse("2024-09-05"), "Daily", 5, 5));

        var expected = new List<BillFromRepositoryDTO>()
        {
            new(2, "company a", 100, DateOnly.Parse("2024-08-30"), "Monthly", "Wages & Salary : Net Pay", 30),
            new(1, "supermarket a", 23, DateOnly.Parse("2024-09-03"), "Weekly", "Groceries", 3),
            new(3, "flight sim", 420, DateOnly.Parse("2024-09-05"), "Daily", "Hobby", 5),
        };

        var actual = await bill.GetAllBills();

        Assert.Equal(expected, actual);
    }
}
