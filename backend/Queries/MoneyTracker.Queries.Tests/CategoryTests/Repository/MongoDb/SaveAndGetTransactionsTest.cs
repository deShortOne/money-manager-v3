using MoneyTracker.Queries.Domain.Entities.Category;
using MoneyTracker.Queries.Infrastructure.Mongo;
using MoneyTracker.Queries.Tests.Fixture;

namespace MoneyTracker.Queries.Tests.CategoryTests.Repository.MongoDb;
public sealed class SaveAndGetCategoriesTest : IClassFixture<MongoDbFixture>
{
    private readonly MongoDbFixture _mongoDbFixture;

    public SaveAndGetCategoriesTest(MongoDbFixture mongoDbFixture)
    {
        _mongoDbFixture = mongoDbFixture;
    }

    [Fact]
    public async Task Test()
    {
        var mongoDb = new MongoDatabase(_mongoDbFixture.ConnectionString);
        var categoriesCache = new CategoryCache(mongoDb);

        var categories = new List<CategoryEntity>
        {
            new(80, "kJUboMlPXM"),
            new(441, "kcdJVUGuPm"),
            new(415, "zXdOgFqRCc"),
        };

        await categoriesCache.SaveCategories(categories);

        var result = await categoriesCache.GetAllCategories();

        Assert.Equal(categories, result);
    }

    [Fact]
    public async Task EmptyAsDifferentUserAttemptsToAccessInformation()
    {
        var mongoDb = new MongoDatabase(_mongoDbFixture.ConnectionString);
        var categoriesCache = new CategoryCache(mongoDb);

        var result = await categoriesCache.GetAllCategories();

        Assert.False(result.IsSuccess);
    }
}
