
using MoneyTracker.DatabaseMigration;
using MoneyTracker.DatabaseMigration.Models;
using MoneyTracker.Shared.Models.RepositoryToService.Bill;

namespace MoneyTracker.Bill.Tests.Repository;
public sealed class GetBillByIdTest : BillRespositoryTestHelper
{
    [Fact]
    public async void GetBillId1()
    {
        Migration.CheckMigration(_postgres.GetConnectionString(), new MigrationOption(true));
        var expected = new BillEntityDTO(1, "supermarket a", 23, new DateOnly(2024, 9, 3), "Weekly", "Groceries", 3, "bank a");

        Assert.Equal(expected, await _billRepo.GetBillById(1));
    }

    [Fact]
    public async void GetBillId2()
    {
        Migration.CheckMigration(_postgres.GetConnectionString(), new MigrationOption(true));
        var expected = new BillEntityDTO(2, "company a", 100, new DateOnly(2024, 8, 30), "Monthly", "Wages & Salary : Net Pay", 30, "bank b");

        Assert.Equal(expected, await _billRepo.GetBillById(2));
    }

    [Fact]
    public async void GetBillId3()
    {
        Migration.CheckMigration(_postgres.GetConnectionString(), new MigrationOption(true));
        var expected = new BillEntityDTO(3, "company a", 100, new DateOnly(2024, 8, 30), "Monthly", "Wages & Salary : Net Pay", 30, "bank a");

        Assert.Equal(expected, await _billRepo.GetBillById(3));
    }
}
