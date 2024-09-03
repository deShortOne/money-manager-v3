
using MoneyTracker.Data.Postgres;
using MoneyTracker.DatabaseMigration;
using MoneyTracker.Shared.Models.Category;
using Testcontainers.PostgreSql;

namespace MoneyTracker.Tests.Database.Postgres
{
    public sealed class CategoryTest : IAsyncLifetime
    {
        private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
#if RUN_LOCAL
            .WithDockerEndpoint("tcp://localhost:2375")
#endif
            .WithImage("postgres:16")
            .WithCleanUp(true)
            .Build();

        public async Task InitializeAsync()
        {
            await _postgres.StartAsync();
            Migration.CheckMigration(_postgres.GetConnectionString());
            return;
        }

        public Task DisposeAsync()
        {
            return _postgres.DisposeAsync().AsTask();
        }

        [Fact]
        public async void FirstLoadCheckTablesThatDataAreThere()
        {
            var db = new PostgresDatabase(_postgres.GetConnectionString());
            var category = new CategoryDatabase(db);

            var expected = new List<CategoryDTO>() {
                new CategoryDTO(1, "Wages & Salary : Net Pay"),
                new CategoryDTO(2, "Bills : Cell Phone"),
                new CategoryDTO(3, "Bills : Rent"),
                new CategoryDTO(4, "Groceries"),
                new CategoryDTO(5, "Hobby"),
            };
            var actual = await category.GetAllCategories();
            TestHelper.CompareLists(expected, actual);
        }

        [Fact]
        public async void AddCategory()
        {
            var db = new PostgresDatabase(_postgres.GetConnectionString());
            var category = new CategoryDatabase(db);

            var categoryToAdd = new CategoryDTO(6, "Speeding tickets");
            await category.AddCategory(new NewCategoryDTO(categoryToAdd.Name));

            var expected = new List<CategoryDTO>() {
                new CategoryDTO(1, "Wages & Salary : Net Pay"),
                new CategoryDTO(2, "Bills : Cell Phone"),
                new CategoryDTO(3, "Bills : Rent"),
                new CategoryDTO(4, "Groceries"),
                new CategoryDTO(5, "Hobby"),
                categoryToAdd,
            };
            var actual = await category.GetAllCategories();
            TestHelper.CompareLists(expected, actual);
        }

        [Fact]
        public async void AddDuplicateCategory()
        {
            var db = new PostgresDatabase(_postgres.GetConnectionString());
            var category = new CategoryDatabase(db);

            var categoryToAdd = new CategoryDTO(6, "Hobby");
            await category.AddCategory(new NewCategoryDTO(categoryToAdd.Name));

            var expected = new List<CategoryDTO>() {
                new CategoryDTO(1, "Wages & Salary : Net Pay"),
                new CategoryDTO(2, "Bills : Cell Phone"),
                new CategoryDTO(3, "Bills : Rent"),
                new CategoryDTO(4, "Groceries"),
                new CategoryDTO(5, "Hobby"),
            };
            var actual = await category.GetAllCategories();
            TestHelper.CompareLists(expected, actual);
        }

        [Fact]
        public async void EditCategory()
        {
            var db = new PostgresDatabase(_postgres.GetConnectionString());
            var category = new CategoryDatabase(db);

            await category.EditCategory(new EditCategoryDTO(5, "Something funky"));

            var expected = new List<CategoryDTO>() {
                new CategoryDTO(1, "Wages & Salary : Net Pay"),
                new CategoryDTO(2, "Bills : Cell Phone"),
                new CategoryDTO(3, "Bills : Rent"),
                new CategoryDTO(4, "Groceries"),
                new CategoryDTO(5, "Something funky"),
            };
            var actual = await category.GetAllCategories();
            TestHelper.CompareLists(expected, actual);
        }

        [Fact]
        public async void EditIntoDuplicateCategory()
        {
            var db = new PostgresDatabase(_postgres.GetConnectionString());
            var category = new CategoryDatabase(db);

            await Assert.ThrowsAsync<Npgsql.PostgresException>(async () =>
                await category.EditCategory(new EditCategoryDTO(4, "Hobby"))
            );

            var expected = new List<CategoryDTO>() {
                new CategoryDTO(1, "Wages & Salary : Net Pay"),
                new CategoryDTO(2, "Bills : Cell Phone"),
                new CategoryDTO(3, "Bills : Rent"),
                new CategoryDTO(4, "Groceries"),
                new CategoryDTO(5, "Hobby"),
            };
            var actual = await category.GetAllCategories();
            TestHelper.CompareLists(expected, actual);
        }

        [Fact]
        public void DeleteCategory()
        {
            // Why would someone want to delete categories?
            // Also constraint prevents deletion
        }
    }
}
