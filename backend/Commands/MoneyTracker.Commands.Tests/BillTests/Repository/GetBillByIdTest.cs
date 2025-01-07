
using MoneyTracker.Commands.DatabaseMigration;
using MoneyTracker.Commands.DatabaseMigration.Models;
using MoneyTracker.Commands.Domain.Entities.Bill;

namespace MoneyTracker.Commands.Tests.BillTests.Repository;
public sealed class GetBillByIdTest : BillRespositoryTestHelper
{
    [Fact]
    public async void GetBillId1()
    {
        Migration.CheckMigration(_postgres.GetConnectionString(), new MigrationOption(true));
        var expected = new BillEntity(1, 11, 23, new DateOnly(2024, 9, 3), 3, "Weekly", 4, 1);

        Assert.Equal(expected, await _billRepo.GetBillById(1));
    }

    [Fact]
    public async void GetBillId2()
    {
        Migration.CheckMigration(_postgres.GetConnectionString(), new MigrationOption(true));
        var expected = new BillEntity(2, 12, 100, new DateOnly(2024, 8, 30), 30, "Monthly", 1, 2);

        Assert.Equal(expected, await _billRepo.GetBillById(2));
    }

    [Fact]
    public async void GetBillId3()
    {
        Migration.CheckMigration(_postgres.GetConnectionString(), new MigrationOption(true));
        var expected = new BillEntity(3, 12, 100, new DateOnly(2024, 8, 30), 30, "Monthly", 1, 3);

        Assert.Equal(expected, await _billRepo.GetBillById(3));
    }

    [Fact]
    public async void GetBillThatDoesntExist()
    {
        Assert.Null(await _billRepo.GetBillById(1));
    }
}
