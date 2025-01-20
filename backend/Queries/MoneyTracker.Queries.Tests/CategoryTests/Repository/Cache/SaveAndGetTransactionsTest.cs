using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Queries.Domain.Entities.Category;
using MoneyTracker.Queries.Domain.Entities.Transaction;
using MoneyTracker.Queries.Infrastructure.Mongo;
using Testcontainers.MongoDb;

namespace MoneyTracker.Queries.Tests.CategoryTests.Repository.Cache;
public sealed class SaveAndGetCategoriesTest : IAsyncLifetime
{
    private readonly MongoDbContainer _mongo = new MongoDbBuilder()
#if RUN_LOCAL
       .WithDockerEndpoint("tcp://localhost:2375")
#endif
       .WithImage("mongo:8")
       .WithCleanUp(true)
       .Build();

    public async Task InitializeAsync()
    {
        await _mongo.StartAsync();

    }

    public Task DisposeAsync()
    {
        return _mongo.DisposeAsync().AsTask();
    }

    [Fact]
    public async Task Test()
    {
        var mongoDb = new MongoDatabase(_mongo.GetConnectionString());
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
        var mongoDb = new MongoDatabase(_mongo.GetConnectionString());
        var categoriesCache = new CategoryCache(mongoDb);

        var result = await categoriesCache.GetAllCategories();

        Assert.False(result.IsSuccess);
    }
}
