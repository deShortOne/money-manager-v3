
namespace MoneyTracker.Queries.Tests.CategoryTests.Repository.DatabaseOnlyRepositoryService;
public class ResetCategoriesCacheTest : DatabaseOnlyTestHelper
{
    [Fact]
    public async Task RefetchDataFromDatabaseAndWriteIntoCache()
    {
        await Assert.ThrowsAsync<NotImplementedException>(() => _categoryRepositoryService.ResetCategoriesCache(CancellationToken.None));
    }
}
