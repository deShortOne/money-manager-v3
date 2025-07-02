using MoneyTracker.Queries.Domain.Entities.Category;
using MoneyTracker.Queries.Infrastructure.Postgres;
using MoneyTracker.Queries.Tests.Fixture;

namespace MoneyTracker.Queries.Tests.CategoryTests.Repository.PostgresDb;
public sealed class GetAllCategoriesTest : IClassFixture<PostgresDbFixture>
{
    private readonly PostgresDbFixture _postgresFixture;

    public GetAllCategoriesTest(PostgresDbFixture postgresFixture)
    {
        _postgresFixture = postgresFixture;
    }

    [Fact]
    public async Task FirstLoadCheckTablesThatDataAreThere()
    {
        var db = new PostgresDatabase(_postgresFixture.ConnectionString);
        var budgetDb = new CategoryDatabase(db);

        var actual = await budgetDb.GetAllCategories(CancellationToken.None);

        var expected = new List<CategoryEntity>()
        {
            new(2, "Bills : Cell Phone"),
            new(3, "Bills : Rent"),
            new(4, "Groceries"),
            new(5, "Hobby"),
            new(6, "Pet Care"),
            new(1, "Wages & Salary : Net Pay"),
        };

        Assert.Equal(expected, actual);
    }
}
