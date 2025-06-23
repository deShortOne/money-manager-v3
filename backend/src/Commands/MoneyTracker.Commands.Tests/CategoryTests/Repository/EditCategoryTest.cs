
using MoneyTracker.Commands.DatabaseMigration;
using MoneyTracker.Commands.DatabaseMigration.Models;
using MoneyTracker.Commands.Domain.Entities.Category;

namespace MoneyTracker.Commands.Tests.CategoryTests.Repository;
public sealed class EditCategoryTest : CategoryRespositoryTestHelper
{
    [Fact]
    public async Task EditCategoryItemIntoDatabase()
    {
        Migration.CheckMigration(_postgres.GetConnectionString(), new MigrationOption(true));

        var id = 1;
        var name = "Super duper new category";
        var editCategoryEntity = new EditCategoryEntity(id, name);
        var CategoryEntityToBe = new CategoryEntity(id, name);

        await _categoryRepo.EditCategory(editCategoryEntity);

        List<CategoryEntity> results = await GetAllCategoryEntities();

        Assert.Multiple(() =>
        {
            Assert.Equal(6, results.Count);
            Assert.Equal(CategoryEntityToBe, results[0]);
        });
    }
}
