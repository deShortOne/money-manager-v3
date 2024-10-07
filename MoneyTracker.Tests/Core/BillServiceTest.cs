using MoneyTracker.Calculation.Bill;
using MoneyTracker.Core;
using MoneyTracker.Data.Postgres;
using MoneyTracker.DatabaseMigration;
using MoneyTracker.DatabaseMigration.Models;
using MoneyTracker.Shared.Auth;
using MoneyTracker.Shared.Core;
using MoneyTracker.Shared.DateManager;
using MoneyTracker.Shared.Models.ControllerToService.Bill;
using MoneyTracker.Shared.Models.RepositoryToService.Bill;
using MoneyTracker.Shared.Models.ServiceToController.Bill;
using MoneyTracker.Shared.Shared;
using MoneyTracker.Tests.Local;
using Moq;
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

    private BillDatabase _billDb;
    private AccountDatabase _accountDb;
    private CategoryDatabase _categoryDb;

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();

        Migration.CheckMigration(_postgres.GetConnectionString(), new MigrationOption(true));

        var db = new PostgresDatabase(_postgres.GetConnectionString());
        _billDb = new BillDatabase(db);
        _accountDb = new AccountDatabase(db);
        _categoryDb = new CategoryDatabase(db);

        return;
    }

    public Task DisposeAsync()
    {
        return _postgres.DisposeAsync().AsTask();
    }
    [Fact]
    public async void FirstLoadCheckTablesThatDataAreThere()
    {
        var user = new AuthenticatedUser(1);
        var expected = new List<BillEntityDTO>()
        {
            new(2, "company a", 100, new DateOnly(2024, 08, 30), "Monthly", "Wages & Salary : Net Pay", 30, "bank b"),
            new(1, "supermarket a", 23, new DateOnly(2024, 09, 03), "Weekly", "Groceries", 3, "bank a"),
        };

        var actual = await _billDb.GetAllBills(user);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async void DeleteBill()
    {
        IDateProvider dateProvider = TestHelper.CreateMockdateProvider(new DateOnly(2024, 8, 24));
        var mockUserAuth = new Mock<IUserAuthenticationService>();
        mockUserAuth.Setup(x => x.DecodeToken(It.IsAny<string>()))
            .Returns(Task.FromResult(new AuthenticatedUser(1)));
        var billService = new BillService(_billDb, dateProvider, mockUserAuth.Object, _accountDb, new IdGenerator(),
            new FrequencyCalculation(), new MonthDayCalculator(), _categoryDb);

        await billService.DeleteBill("", new DeleteBillRequestDTO(1));

        var expected = new List<BillResponseDTO>()
        {
            new(2, "company a", 100, new DateOnly(2024, 08, 30), "Monthly", "Wages & Salary : Net Pay", null, "bank b"),
        };

        var actual = await billService.GetAllBills("");

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async void EditBill()
    {
        IDateProvider dateProvider = TestHelper.CreateMockdateProvider(new DateOnly(2024, 8, 24));
        var mockUserAuth = new Mock<IUserAuthenticationService>();
        mockUserAuth.Setup(x => x.DecodeToken(It.IsAny<string>()))
            .Returns(Task.FromResult(new AuthenticatedUser(1)));
        var billService = new BillService(_billDb, dateProvider, mockUserAuth.Object, _accountDb, new IdGenerator(),
            new FrequencyCalculation(), new MonthDayCalculator(), _categoryDb);

        await billService.EditBill("", new EditBillRequestDTO(1, payee: "supermarket b"));

        var expected = new List<BillResponseDTO>()
        {
            new(2, "company a", 100, new DateOnly(2024, 08, 30), "Monthly", "Wages & Salary : Net Pay", null, "bank b"),
            new(1, "supermarket b", 23, new DateOnly(2024, 09, 03), "Weekly", "Groceries", null, "bank a"),
        };

        var actual = await billService.GetAllBills("");

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async void AddBill()
    {
        IDateProvider dateProvider = TestHelper.CreateMockdateProvider(new DateOnly(2024, 8, 24));
        var mockUserAuth = new Mock<IUserAuthenticationService>();
        mockUserAuth.Setup(x => x.DecodeToken(It.IsAny<string>()))
            .Returns(Task.FromResult(new AuthenticatedUser(1)));
        var billService = new BillService(_billDb, dateProvider, mockUserAuth.Object, _accountDb, new IdGenerator(),
            new FrequencyCalculation(), new MonthDayCalculator(), _categoryDb);

        await billService.AddBill("", new NewBillRequestDTO("flight sim", 420, new DateOnly(2024, 09, 05), "Daily", 5, 1));

        var expected = new List<BillResponseDTO>()
        {
            new(2, "company a", 100, new DateOnly(2024, 08, 30), "Monthly", "Wages & Salary : Net Pay", null, "bank b"),
            new(1, "supermarket a", 23, new DateOnly(2024, 09, 03), "Weekly", "Groceries", null, "bank a"),
            new(4, "flight sim", 420, new DateOnly(2024, 09, 05), "Daily", "Hobby", null, "bank a"),
        };

        var actual = await billService.GetAllBills("");

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async void FirstLoadCheckButCurrentDateIsOneIterationAfterDueDate()
    {
        IDateProvider dateProvider = TestHelper.CreateMockdateProvider(new DateOnly(2024, 9, 7));
        var mockUserAuth = new Mock<IUserAuthenticationService>();
        mockUserAuth.Setup(x => x.DecodeToken(It.IsAny<string>()))
            .Returns(Task.FromResult(new AuthenticatedUser(1)));
        var billService = new BillService(_billDb, dateProvider, mockUserAuth.Object, _accountDb, new IdGenerator(),
            new FrequencyCalculation(), new MonthDayCalculator(), _categoryDb);

        var expected = new List<BillResponseDTO>()
        {
            new(2, "company a", 100, new DateOnly(2024, 08, 30), "Monthly", "Wages & Salary : Net Pay",
                new OverDueBillInfo(8, [new DateOnly(2024, 8, 30)]), "bank b"),
            new(1, "supermarket a", 23, new DateOnly(2024, 09, 03), "Weekly", "Groceries",
                new OverDueBillInfo(4, [new DateOnly(2024, 9, 3)]), "bank a"),
        };

        var actual = await billService.GetAllBills("");

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async void FirstLoadCheckButCurrentDateIsMultipleIterationsAfterDueDate()
    {
        IDateProvider dateProvider = TestHelper.CreateMockdateProvider(new DateOnly(2024, 10, 4));
        var mockUserAuth = new Mock<IUserAuthenticationService>();
        mockUserAuth.Setup(x => x.DecodeToken(It.IsAny<string>()))
            .Returns(Task.FromResult(new AuthenticatedUser(1)));
        var billService = new BillService(_billDb, dateProvider, mockUserAuth.Object, _accountDb, new IdGenerator(),
            new FrequencyCalculation(), new MonthDayCalculator(), _categoryDb);

        DateOnly[] dates = [new DateOnly(2024, 9, 3), new DateOnly(2024, 9, 10), new DateOnly(2024, 9, 17),
            new DateOnly(2024, 9, 24), new DateOnly(2024, 10, 1)];
        var expected = new List<BillResponseDTO>()
        {
            new(2, "company a", 100, new DateOnly(2024, 08, 30), "Monthly", "Wages & Salary : Net Pay",
                new OverDueBillInfo(35, [new DateOnly(2024, 8, 30), new DateOnly(2024, 9, 30)]), "bank b"),
            new(1, "supermarket a", 23, new DateOnly(2024, 09, 03), "Weekly", "Groceries",
                new OverDueBillInfo(31, dates), "bank a"),
        };

        var actual = await billService.GetAllBills("");

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async void FirstLoadCheckButCurrentDateIsMultipleIterationsAfterDueDate2()
    {
        IDateProvider dateProvider = TestHelper.CreateMockdateProvider(new DateOnly(2024, 10, 3));
        var mockUserAuth = new Mock<IUserAuthenticationService>();
        mockUserAuth.Setup(x => x.DecodeToken(It.IsAny<string>()))
            .Returns(Task.FromResult(new AuthenticatedUser(1)));
        var billService = new BillService(_billDb, dateProvider, mockUserAuth.Object, _accountDb, new IdGenerator(),
            new FrequencyCalculation(), new MonthDayCalculator(), _categoryDb);

        DateOnly[] dates = [new DateOnly(2024, 9, 3), new DateOnly(2024, 9, 10), new DateOnly(2024, 9, 17),
            new DateOnly(2024, 9, 24), new DateOnly(2024, 10, 1)];
        var expected = new List<BillResponseDTO>()
        {
            new(2, "company a", 100, new DateOnly(2024, 08, 30), "Monthly", "Wages & Salary : Net Pay",
                new OverDueBillInfo(34, [new DateOnly(2024, 8, 30), new DateOnly(2024, 9, 30)]), "bank b"),
            new(1, "supermarket a", 23, new DateOnly(2024, 09, 03), "Weekly", "Groceries",
                new OverDueBillInfo(30, dates), "bank a"),
        };

        var actual = await billService.GetAllBills("");

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async void SkipOccurence_SkipTwoOccurences_BillIsSkippedPassed()
    {
        IDateProvider dateProvider = TestHelper.CreateMockdateProvider(new DateOnly(2024, 10, 3));
        var mockUserAuth = new Mock<IUserAuthenticationService>();
        mockUserAuth.Setup(x => x.DecodeToken(It.IsAny<string>()))
            .Returns(Task.FromResult(new AuthenticatedUser(1)));
        var billService = new BillService(_billDb, dateProvider, mockUserAuth.Object, _accountDb, new IdGenerator(),
            new FrequencyCalculation(), new MonthDayCalculator(), _categoryDb);

        await billService.SkipOccurence("", new SkipBillOccurrenceRequestDTO(1,
            new DateOnly(2024, 9, 17)));

        DateOnly[] dates = [new DateOnly(2024, 9, 24), new DateOnly(2024, 10, 1)];
        var expected = new List<BillResponseDTO>()
        {
            new(2, "company a", 100, new DateOnly(2024, 08, 30), "Monthly", "Wages & Salary : Net Pay",
                new OverDueBillInfo(34, [new DateOnly(2024, 8, 30), new DateOnly(2024, 9, 30)]), "bank b"),
            new(1, "supermarket a", 23, new DateOnly(2024, 9, 24), "Weekly", "Groceries",
                new OverDueBillInfo(9, dates), "bank a"),
        };

        var actual = await billService.GetAllBills("");
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async void EditBill_EditBillNotAssoicatedWithUser_ThrowsError()
    {
        IDateProvider dateProvider = TestHelper.CreateMockdateProvider(new DateOnly(2024, 10, 3));
        var mockUserAuth = new Mock<IUserAuthenticationService>();
        mockUserAuth.Setup(x => x.DecodeToken(It.IsAny<string>()))
            .Returns(Task.FromResult(new AuthenticatedUser(2)));
        var billService = new BillService(_billDb, dateProvider, mockUserAuth.Object, _accountDb, new IdGenerator(),
            new FrequencyCalculation(), new MonthDayCalculator(), _categoryDb);

        var editBillMessage = await Assert.ThrowsAsync<InvalidDataException>(async () =>
        {
            await billService.EditBill("", new EditBillRequestDTO(1, payee: "supermarket b"));
        });
        Assert.Equal("Bill not found", editBillMessage.Message);
    }

    [Fact]
    public async void DeleteBill_EditBillNotAssoicatedWithUser_ThrowsError()
    {
        IDateProvider dateProvider = TestHelper.CreateMockdateProvider(new DateOnly(2024, 10, 3));
        var mockUserAuth = new Mock<IUserAuthenticationService>();
        mockUserAuth.Setup(x => x.DecodeToken(It.IsAny<string>()))
            .Returns(Task.FromResult(new AuthenticatedUser(2)));
        var billService = new BillService(_billDb, dateProvider, mockUserAuth.Object, _accountDb, new IdGenerator(),
            new FrequencyCalculation(), new MonthDayCalculator(), _categoryDb);

        var editBillMessage = await Assert.ThrowsAsync<InvalidDataException>(async () =>
        {
            await billService.DeleteBill("", new DeleteBillRequestDTO(1));
        });
        Assert.Equal("Bill not found", editBillMessage.Message);
    }

    [Fact]
    public async void SkipOccurence_EditBillNotAssoicatedWithUser_ThrowsError()
    {
        IDateProvider dateProvider = TestHelper.CreateMockdateProvider(new DateOnly(2024, 10, 3));
        var mockUserAuth = new Mock<IUserAuthenticationService>();
        mockUserAuth.Setup(x => x.DecodeToken(It.IsAny<string>()))
            .Returns(Task.FromResult(new AuthenticatedUser(2)));
        var billService = new BillService(_billDb, dateProvider, mockUserAuth.Object, _accountDb, new IdGenerator(),
            new FrequencyCalculation(), new MonthDayCalculator(), _categoryDb);

        var editBillMessage = await Assert.ThrowsAsync<InvalidDataException>(async () =>
        {
            await billService.SkipOccurence("", new SkipBillOccurrenceRequestDTO(1, new DateOnly(2024, 9, 17)));
        });
        Assert.Equal("Bill not found", editBillMessage.Message);
    }

    [Fact]
    public async void AddBill_AddBillWithAccountNotAssoicatedWithUser_ThrowsError()
    {
        IDateProvider dateProvider = TestHelper.CreateMockdateProvider(new DateOnly(2024, 10, 3));
        var mockUserAuth = new Mock<IUserAuthenticationService>();
        mockUserAuth.Setup(x => x.DecodeToken(It.IsAny<string>()))
            .Returns(Task.FromResult(new AuthenticatedUser(2)));
        var billService = new BillService(_billDb, dateProvider, mockUserAuth.Object, _accountDb, new IdGenerator(),
            new FrequencyCalculation(), new MonthDayCalculator(), _categoryDb);

        var addBillMessage = await Assert.ThrowsAsync<InvalidDataException>(async () =>
        {
            await billService.AddBill("", new NewBillRequestDTO("", 0, new DateOnly(), "", 1, 1));
        });
        Assert.Equal("Account not found", addBillMessage.Message);
    }

    [Fact]
    public async void EditBill_EditBillToAccountNotAssoicatedWithUser_ThrowsError()
    {
        IDateProvider dateProvider = TestHelper.CreateMockdateProvider(new DateOnly(2024, 10, 3));
        var mockUserAuth = new Mock<IUserAuthenticationService>();
        mockUserAuth.Setup(x => x.DecodeToken(It.IsAny<string>()))
            .Returns(Task.FromResult(new AuthenticatedUser(2)));
        var billService = new BillService(_billDb, dateProvider, mockUserAuth.Object, _accountDb, new IdGenerator(),
            new FrequencyCalculation(), new MonthDayCalculator(), _categoryDb);

        var addBillMessage = await Assert.ThrowsAsync<InvalidDataException>(async () =>
        {
            await billService.EditBill("", new EditBillRequestDTO(3, accountId: 1));
        });
        Assert.Equal("Account not found", addBillMessage.Message);
    }
}
