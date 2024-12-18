
using MoneyTracker.Commands.DatabaseMigration;
using MoneyTracker.Commands.DatabaseMigration.Models;

namespace MoneyTracker.Commands.Tests.CategoryTests.Repository;
public sealed class GetLastCategoryIdTest : CategoryRespositoryTestHelper
{
    [Fact]
    public void GetLastCategoryId()
    {
        Migration.CheckMigration(_postgres.GetConnectionString(), new MigrationOption(true));

        Assert.Multiple(async () =>
        {
            Assert.Equal(6, await _categoryRepo.GetLastCategoryId());
        });
    }
}
