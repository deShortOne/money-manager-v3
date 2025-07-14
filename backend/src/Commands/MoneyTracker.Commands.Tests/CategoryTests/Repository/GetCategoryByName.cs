
using MoneyTracker.Commands.Domain.Entities.Category;
using MoneyTracker.Commands.Infrastructure.Postgres;
using MoneyTracker.Commands.Tests.Fixture;

namespace MoneyTracker.Commands.Tests.CategoryTests.Repository;
public class GetCategoryByName : IClassFixture<PostgresDbFixture>
{
    private CategoryCommandRepository _categoryRepo;

    public GetCategoryByName(PostgresDbFixture postgresFixture)
    {
        var _database = new PostgresDatabase(postgresFixture.ConnectionString);
        _categoryRepo = new CategoryCommandRepository(_database);
    }

    [Fact]
    public void GetLastCategoryId()
    {

        Assert.Multiple(async () =>
        {
            Assert.Equal(new CategoryEntity(1, "Wages & Salary : Net Pay"), await _categoryRepo.GetCategory("Wages & Salary : Net Pay", CancellationToken.None));
            Assert.Equal(new CategoryEntity(2, "Bills : Cell Phone"), await _categoryRepo.GetCategory("Bills : Cell Phone", CancellationToken.None));
            Assert.Equal(new CategoryEntity(3, "Bills : Rent"), await _categoryRepo.GetCategory("Bills : Rent", CancellationToken.None));
            Assert.Equal(new CategoryEntity(4, "Groceries"), await _categoryRepo.GetCategory("Groceries", CancellationToken.None));
            Assert.Equal(new CategoryEntity(5, "Hobby"), await _categoryRepo.GetCategory("Hobby", CancellationToken.None));
            Assert.Equal(new CategoryEntity(6, "Pet Care"), await _categoryRepo.GetCategory("Pet Care", CancellationToken.None));
        });
    }

    [Fact]
    public void FailGetLastCategoryName()
    {
        Assert.Multiple(async () =>
        {
            Assert.Null(await _categoryRepo.GetCategory("a category that doesn't exist", CancellationToken.None));
            Assert.Null(await _categoryRepo.GetCategory("-1", CancellationToken.None));
        });
    }
}
