using MoneyTracker.Core;
using MoneyTracker.Data.Postgres;
using MoneyTracker.DatabaseMigration;
using MoneyTracker.DatabaseMigration.Models;
using MoneyTracker.Shared.DateManager;
using MoneyTracker.Shared.Models.Bill;
using MoneyTracker.Tests.Local;
using Testcontainers.PostgreSql;

namespace MoneyTracker.Tests.Core;
public class BillServiceTest : IAsyncLifetime
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
        IDateProvider dateProvider = TestHelper.CreateMockdateProvider(new DateOnly(2024, 8, 24));
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
        IDateProvider dateProvider = TestHelper.CreateMockdateProvider(new DateOnly(2024, 8, 24));
        var bill = new BillDatabase(db);

        var billService = new BillService(bill, dateProvider);
        await billService.DeleteBill(new DeleteBillDTO(1));

        var expected = new List<BillDTO>()
        {
            new BillDTO(2, "company a", 100, DateOnly.Parse("2024-08-30"), "Monthly", "Wages & Salary : Net Pay", null),
        };

        var actual = await billService.GetAllBills();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async void EditBill()
    {
        var db = new PostgresDatabase(_postgres.GetConnectionString());
        var bill = new BillDatabase(db);

        IDateProvider dateProvider = TestHelper.CreateMockdateProvider(new DateOnly(2024, 8, 24));
        var billService = new BillService(bill, dateProvider);
        await billService.EditBill(new EditBillDTO(1, payee: "supermarket b"));

        var expected = new List<BillDTO>()
        {
            new BillDTO(2, "company a", 100, DateOnly.Parse("2024-08-30"), "Monthly", "Wages & Salary : Net Pay", null),
            new BillDTO(1, "supermarket b", 23, DateOnly.Parse("2024-09-03"), "Weekly", "Groceries", null),
        };

        var actual = await billService.GetAllBills();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async void AddBill()
    {
        var db = new PostgresDatabase(_postgres.GetConnectionString());
        var bill = new BillDatabase(db);

        IDateProvider dateProvider = TestHelper.CreateMockdateProvider(new DateOnly(2024, 8, 24));
        var billService = new BillService(bill, dateProvider);
        await billService.AddBill(new NewBillDTO("flight sim", 420, DateOnly.Parse("2024-09-05"), "Daily", 5));

        var expected = new List<BillDTO>()
        {
            new BillDTO(2, "company a", 100, DateOnly.Parse("2024-08-30"), "Monthly", "Wages & Salary : Net Pay", null),
            new BillDTO(1, "supermarket a", 23, DateOnly.Parse("2024-09-03"), "Weekly", "Groceries", null),
            new BillDTO(3, "flight sim", 420, DateOnly.Parse("2024-09-05"), "Daily", "Hobby", null),
        };

        var actual = await billService.GetAllBills();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async void FirstLoadCheckButCurrentDateIsOneIterationAfterDueDate()
    {
        var db = new PostgresDatabase(_postgres.GetConnectionString());
        var bill = new BillDatabase(db);

        IDateProvider dateProvider = TestHelper.CreateMockdateProvider(new DateOnly(2024, 9, 7));
        var billService = new BillService(bill, dateProvider);

        var expected = new List<BillDTO>()
        {
            new BillDTO(2, "company a", 100, DateOnly.Parse("2024-08-30"), "Monthly", "Wages & Salary : Net Pay",
                new OverDueBillInfo(8, 1)),
            new BillDTO(1, "supermarket a", 23, DateOnly.Parse("2024-09-03"), "Weekly", "Groceries",
                new OverDueBillInfo(4, 1)),
        };

        var actual = await billService.GetAllBills();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async void FirstLoadCheckButCurrentDateIsMultipleIterationsAfterDueDate()
    {
        var db = new PostgresDatabase(_postgres.GetConnectionString());
        var bill = new BillDatabase(db);

        IDateProvider dateProvider = TestHelper.CreateMockdateProvider(new DateOnly(2024, 10, 4));
        var billService = new BillService(bill, dateProvider);

        var expected = new List<BillDTO>()
        {
            new BillDTO(2, "company a", 100, DateOnly.Parse("2024-08-30"), "Monthly", "Wages & Salary : Net Pay",
                new OverDueBillInfo(35, 2)),
            new BillDTO(1, "supermarket a", 23, DateOnly.Parse("2024-09-03"), "Weekly", "Groceries",
                new OverDueBillInfo(31, 5)),
        };

        var actual = await billService.GetAllBills();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async void FirstLoadCheckButCurrentDateIsMultipleIterationsAfterDueDate2()
    {
        var db = new PostgresDatabase(_postgres.GetConnectionString());
        var bill = new BillDatabase(db);

        IDateProvider dateProvider = TestHelper.CreateMockdateProvider(new DateOnly(2024, 10, 3));
        var billService = new BillService(bill, dateProvider);

        var expected = new List<BillDTO>()
        {
            new BillDTO(2, "company a", 100, DateOnly.Parse("2024-08-30"), "Monthly", "Wages & Salary : Net Pay",
                new OverDueBillInfo(34, 2)),
            new BillDTO(1, "supermarket a", 23, DateOnly.Parse("2024-09-03"), "Weekly", "Groceries",
                new OverDueBillInfo(30, 5)),
        };

        var actual = await billService.GetAllBills();

        Assert.Equal(expected, actual);
    }
}
