using MoneyTracker.Commands.Infrastructure.Postgres;
using MoneyTracker.Commands.Tests.Fixture;

namespace MoneyTracker.Commands.Tests.CategoryTests.Repository;
public sealed class GetLastCategoryIdTest : IClassFixture<PostgresDbFixture>
{
    private CategoryCommandRepository _categoryRepo;

    public GetLastCategoryIdTest(PostgresDbFixture postgresFixture)
    {
        var _database = new PostgresDatabase(postgresFixture.ConnectionString);
        _categoryRepo = new CategoryCommandRepository(_database);
    }

    [Fact]
    public async Task GetLastCategoryId()
    {
        Assert.Equal(6, await _categoryRepo.GetLastCategoryId(CancellationToken.None));
    }
}
