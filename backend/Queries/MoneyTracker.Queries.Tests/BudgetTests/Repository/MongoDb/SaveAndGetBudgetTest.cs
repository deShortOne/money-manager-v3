using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Queries.Domain.Entities.BudgetCategory;
using MoneyTracker.Queries.Infrastructure.Mongo;
using MoneyTracker.Queries.Tests.Fixture;

namespace MoneyTracker.Queries.Tests.BudgetTests.Repository.MongoDb;
public sealed class SaveAndGetBudgetTest : IClassFixture<MongoDbFixture>
{
    private readonly MongoDbFixture _mongoDbFixture;

    public SaveAndGetBudgetTest(MongoDbFixture mongoDbFixture)
    {
        _mongoDbFixture = mongoDbFixture;
    }

    [Fact]
    public async Task Test()
    {
        var mongoDb = new MongoDatabase(_mongoDbFixture.ConnectionString);
        var budgetCache = new BudgetCache(mongoDb);

        var authedUser = new AuthenticatedUser(6423);
        var budgetGroup = new List<BudgetGroupEntity>
        {
            new(556, "YqrvhQAmIH", 608, 714, 637, [
                new(407, "dixAiQcKOX", 286, 893, 200)
            ]),
            new(451, "SnUWQWDZBE", 222, 576, 302, [
                new(617, "CTAXddZlRg", 483, 226, 604),
                new(918, "cEKNZhrDxZ", 819, 927, 272),
                new(653, "PrQfsMtiMv", 200, 502, 47),
                new(336, "paWBXMiQLr", 508, 45, 610),
            ]),
            new(607, "rBSStnXVqF", 329, 580, 99, []),
        };

        await budgetCache.SaveBudget(authedUser, budgetGroup);

        var result = await budgetCache.GetBudget(authedUser);

        Assert.Equal(budgetGroup, result);
    }

    [Fact]
    public async Task EmptyAsDifferentUserAttemptsToAccessInformation()
    {
        var mongoDb = new MongoDatabase(_mongoDbFixture.ConnectionString);
        var budgetCache = new BudgetCache(mongoDb);

        var authedUser1 = new AuthenticatedUser(2);
        var authedUser2 = new AuthenticatedUser(3);
        var budgetGroup = new List<BudgetGroupEntity>
        {
            new(556, "YqrvhQAmIH", 608, 714, 637, [new(407, "dixAiQcKOX", 286, 893, 200)]),
            new(451, "SnUWQWDZBE", 222, 576, 302, [
                new(617, "CTAXddZlRg", 483, 226, 604),
                new(918, "cEKNZhrDxZ", 819, 927, 272),
                new(653, "PrQfsMtiMv", 200, 502, 47),
                new(336, "paWBXMiQLr", 508, 45, 610),
            ]),
            new(607, "rBSStnXVqF", 329, 580, 99, []),
        };

        await budgetCache.SaveBudget(authedUser1, budgetGroup);

        var result = await budgetCache.GetBudget(authedUser2);

        Assert.False(result.IsSuccess);
    }
}
