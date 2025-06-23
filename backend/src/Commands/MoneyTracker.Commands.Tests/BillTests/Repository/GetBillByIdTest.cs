using MoneyTracker.Commands.Domain.Entities.Bill;
using MoneyTracker.Commands.Infrastructure.Postgres;
using MoneyTracker.Commands.Tests.Fixture;

namespace MoneyTracker.Commands.Tests.BillTests.Repository;
public sealed class GetBillByIdTest : IClassFixture<PostgresDbFixture>
{
    private BillCommandRepository _billRepo;

    public GetBillByIdTest(PostgresDbFixture postgresFixture)
    {
        var _database = new PostgresDatabase(postgresFixture.ConnectionString);
        _billRepo = new BillCommandRepository(_database);
    }

    [Fact]
    public async Task GetBillId1()
    {
        var expected = new BillEntity(1, 7, 23, new DateOnly(2024, 9, 3), 3, "Weekly", 4, 1);

        Assert.Equal(expected, await _billRepo.GetBillById(1));
    }

    [Fact]
    public async Task GetBillId2()
    {
        var expected = new BillEntity(2, 9, 100, new DateOnly(2024, 8, 30), 30, "Monthly", 1, 3);

        Assert.Equal(expected, await _billRepo.GetBillById(2));
    }

    [Fact]
    public async Task GetBillId3()
    {
        var expected = new BillEntity(3, 17, 100, new DateOnly(2024, 8, 30), 30, "Monthly", 1, 2);

        Assert.Equal(expected, await _billRepo.GetBillById(3));
    }

    [Fact]
    public async Task GetBillThatDoesntExist()
    {
        Assert.Null(await _billRepo.GetBillById(-1));
    }
}
