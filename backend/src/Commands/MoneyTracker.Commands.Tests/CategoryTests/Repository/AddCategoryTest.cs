
using MoneyTracker.Commands.DatabaseMigration;
using MoneyTracker.Commands.DatabaseMigration.Models;
using MoneyTracker.Commands.Domain.Entities.Category;

namespace MoneyTracker.Commands.Tests.CategoryTests.Repository;
public sealed class AddCategoryTest : CategoryRespositoryTestHelper
{
    [Fact]
    public async Task AddCategoryItemIntoDatabase()
    {
        Migration.CheckMigration(_postgres.GetConnectionString(), new MigrationOption(true));

        var id = 117;
        var name = "Super duper new category";
        var newCategoryEntity = new CategoryEntity(id, name);

        await _categoryRepo.AddCategory(newCategoryEntity, CancellationToken.None);

        List<CategoryEntity> results = await GetAllCategoryEntities();

        Assert.Multiple(() =>
        {
            Assert.Equal(7, results.Count);
            Assert.Equal(newCategoryEntity, results[6]);
        });
    }
}
