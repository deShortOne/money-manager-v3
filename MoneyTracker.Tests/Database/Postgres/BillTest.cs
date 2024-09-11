
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

        var expected = new List<BillDTO>()
        {
            new BillDTO(2, "company a", 100, DateOnly.Parse("2024-08-30"), "Monthly", "Wages & Salary : Net Pay", null),
            new BillDTO(1, "supermarket a", 23, DateOnly.Parse("2024-09-03"), "Weekly", "Groceries", null),
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

        var expected = new List<BillDTO>()
        {
            new BillDTO(2, "company a", 100, DateOnly.Parse("2024-08-30"), "Monthly", "Wages & Salary : Net Pay", null),
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

        var expected = new List<BillDTO>()
        {
            new BillDTO(2, "company a", 100, DateOnly.Parse("2024-08-30"), "Monthly", "Wages & Salary : Net Pay", null),
            new BillDTO(1, "supermarket b", 23, DateOnly.Parse("2024-09-03"), "Weekly", "Groceries", null),
        };

        var actual = await bill.GetAllBills();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async void AddBill()
    {
        var db = new PostgresDatabase(_postgres.GetConnectionString());
        var bill = new BillDatabase(db);
        await bill.AddBill(new NewBillDTO("flight sim", 420, DateOnly.Parse("2024-09-05"), "Daily", 5));

        var expected = new List<BillDTO>()
        {
            new BillDTO(2, "company a", 100, DateOnly.Parse("2024-08-30"), "Monthly", "Wages & Salary : Net Pay", null),
            new BillDTO(1, "supermarket a", 23, DateOnly.Parse("2024-09-03"), "Weekly", "Groceries", null),
            new BillDTO(3, "flight sim", 420, DateOnly.Parse("2024-09-05"), "Daily", "Hobby", null),
        };

        var actual = await bill.GetAllBills();

        Assert.Equal(expected, actual);
    }
}
