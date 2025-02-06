using MoneyTracker.Commands.DatabaseMigration;
using MoneyTracker.Commands.DatabaseMigration.Models;

namespace MoneyTracker.Commands.Tests.BillTests.Repository;
public sealed class GetLastIdTest : BillRespositoryTestHelper
{
    [Fact]
    public async Task GetLastIdWithNoDataInTables()
    {
        Assert.Equal(0, await _billRepo.GetLastId());
    }

    [Fact]
    public async Task GetLastIdWithDataInTables()
    {
        Migration.CheckMigration(_postgres.GetConnectionString(), new MigrationOption(true));

        Assert.Equal(3, await _billRepo.GetLastId());
    }
}
