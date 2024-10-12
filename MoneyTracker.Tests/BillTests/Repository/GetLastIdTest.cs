
using MoneyTracker.DatabaseMigration;
using MoneyTracker.DatabaseMigration.Models;

namespace MoneyTracker.Tests.BillTests.Repository;
public sealed class GetLastIdTest : BillRespositoryTestHelper
{
    [Fact]
    public async void GetLastIdWithNoDataInTables()
    {
        Assert.Equal(0, await _billRepo.GetLastId());
    }

    [Fact]
    public async void GetLastIdWithDataInTables()
    {
        Migration.CheckMigration(_postgres.GetConnectionString(), new MigrationOption(true));

        Assert.Equal(3, await _billRepo.GetLastId());
    }
}
