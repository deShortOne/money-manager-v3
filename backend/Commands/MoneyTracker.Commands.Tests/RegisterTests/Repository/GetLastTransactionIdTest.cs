
using MoneyTracker.Commands.DatabaseMigration;
using MoneyTracker.Commands.DatabaseMigration.Models;
using MoneyTracker.Commands.Tests.RegisterTests.Repository;

namespace MoneyTracker.Commands.Tests.TransactionTests.Repository;
public sealed class GetLastTransactionIdTest : RegisterRespositoryTestHelper
{
    [Fact]
    public void GetLastTransactionId()
    {
        Migration.CheckMigration(_postgres.GetConnectionString(), new MigrationOption(true));

        Assert.Multiple(async () =>
        {
            Assert.Equal(10, await _registerRepo.GetLastTransactionId());
        });
    }
}
