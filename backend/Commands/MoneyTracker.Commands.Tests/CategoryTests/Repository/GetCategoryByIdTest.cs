
using MoneyTracker.Commands.DatabaseMigration;
using MoneyTracker.Commands.DatabaseMigration.Models;
using MoneyTracker.Commands.Domain.Entities.Category;

namespace MoneyTracker.Commands.Tests.CategoryTests.Repository;
public sealed class GetCategoryByIdTest : CategoryRespositoryTestHelper
{
    [Fact]
    public void GetLastCategoryId()
    {
        Migration.CheckMigration(_postgres.GetConnectionString(), new MigrationOption(true));

        Assert.Multiple(async () =>
        {
            Assert.Equal(new CategoryEntity(1, "Wages & Salary : Net Pay"), await _categoryRepo.GetCategory(1));
            Assert.Equal(new CategoryEntity(2, "Bills : Cell Phone"), await _categoryRepo.GetCategory(2));
            Assert.Equal(new CategoryEntity(3, "Bills : Rent"), await _categoryRepo.GetCategory(3));
            Assert.Equal(new CategoryEntity(4, "Groceries"), await _categoryRepo.GetCategory(4));
            Assert.Equal(new CategoryEntity(5, "Hobby"), await _categoryRepo.GetCategory(5));
            Assert.Equal(new CategoryEntity(6, "Pet Care"), await _categoryRepo.GetCategory(6));
        });
    }

    [Fact]
    public void FailGetLastCategoryId()
    {
        Migration.CheckMigration(_postgres.GetConnectionString(), new MigrationOption(true));

        Assert.Multiple(async () =>
        {
            Assert.Null(await _categoryRepo.GetCategory(70));
            Assert.Null(await _categoryRepo.GetCategory(-1));
        });
    }
}
