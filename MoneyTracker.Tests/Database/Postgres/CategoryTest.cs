
using DatabaseMigration;
using MoneyTracker.Data.Postgres;
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
            var db = new Helper(_postgres.GetConnectionString());
            var category = new Category(db);

            var expected = new List<CategoryDTO>() {
                new CategoryDTO() {
                    Id = 1,
                    Name = "Wages & Salary : Net Pay"
                },
                new CategoryDTO() {
                    Id = 2,
                    Name = "Bills : Cell Phone"
                },
                new CategoryDTO() {
                    Id = 3,
                    Name = "Bills : Rent"
                },
                new CategoryDTO() {
                    Id = 4,
                    Name = "Groceries"
                },
                new CategoryDTO() {
                    Id = 5,
                    Name = "Hobby"
                },
            };
            var actual = await category.GetAllCategories();
            TestHelper.CompareLists(expected, actual);
        }

        [Fact]
        public async void AddCategory()
        {
            var db = new Helper(_postgres.GetConnectionString());
            var category = new Category(db);

            var categoryToAdd = new CategoryDTO()
            {
                Id = 6,
                Name = "Speeding tickets",
            };
            await category.AddCategory(new NewCategoryDTO()
            {
                Name = categoryToAdd.Name,
            });

            var expected = new List<CategoryDTO>() {
                new CategoryDTO() {
                    Id = 1,
                    Name = "Wages & Salary : Net Pay"
                },
                new CategoryDTO() {
                    Id = 2,
                    Name = "Bills : Cell Phone"
                },
                new CategoryDTO() {
                    Id = 3,
                    Name = "Bills : Rent"
                },
                new CategoryDTO() {
                    Id = 4,
                    Name = "Groceries"
                },
                new CategoryDTO() {
                    Id = 5,
                    Name = "Hobby"
                },
                categoryToAdd,
            };
            var actual = await category.GetAllCategories();
            TestHelper.CompareLists(expected, actual);
        }

        [Fact]
        public async void AddDuplicateCategory()
        {
            var db = new Helper(_postgres.GetConnectionString());
            var category = new Category(db);

            var categoryToAdd = new CategoryDTO()
            {
                Id = 6,
                Name = "Hobby",
            };
            await category.AddCategory(new NewCategoryDTO()
            {
                Name = categoryToAdd.Name,
            });

            var expected = new List<CategoryDTO>() {
                new CategoryDTO() {
                    Id = 1,
                    Name = "Wages & Salary : Net Pay"
                },
                new CategoryDTO() {
                    Id = 2,
                    Name = "Bills : Cell Phone"
                },
                new CategoryDTO() {
                    Id = 3,
                    Name = "Bills : Rent"
                },
                new CategoryDTO() {
                    Id = 4,
                    Name = "Groceries"
                },
                new CategoryDTO() {
                    Id = 5,
                    Name = "Hobby"
                },
            };
            var actual = await category.GetAllCategories();
            TestHelper.CompareLists(expected, actual);
        }

        [Fact]
        public async void EditCategory()
        {
            var db = new Helper(_postgres.GetConnectionString());
            var category = new Category(db);

            await category.EditCategory(new EditCategoryDTO()
            {
                Id = 5,
                Name = "Something funky",
            });

            var expected = new List<CategoryDTO>() {
                new CategoryDTO() {
                    Id = 1,
                    Name = "Wages & Salary : Net Pay"
                },
                new CategoryDTO() {
                    Id = 2,
                    Name = "Bills : Cell Phone"
                },
                new CategoryDTO() {
                    Id = 3,
                    Name = "Bills : Rent"
                },
                new CategoryDTO() {
                    Id = 4,
                    Name = "Groceries"
                },
                new CategoryDTO() {
                    Id = 5,
                    Name = "Something funky"
                },
            };
            var actual = await category.GetAllCategories();
            TestHelper.CompareLists(expected, actual);
        }

        [Fact]
        public async void EditIntoDuplicateCategory()
        {
            var db = new Helper(_postgres.GetConnectionString());
            var category = new Category(db);

            await Assert.ThrowsAsync<Npgsql.PostgresException>(async () =>
                await category.EditCategory(new EditCategoryDTO()
                {
                    Id = 4,
                    Name = "Hobby",
                })
            );

            var expected = new List<CategoryDTO>() {
                new CategoryDTO() {
                    Id = 1,
                    Name = "Wages & Salary : Net Pay"
                },
                new CategoryDTO() {
                    Id = 2,
                    Name = "Bills : Cell Phone"
                },
                new CategoryDTO() {
                    Id = 3,
                    Name = "Bills : Rent"
                },
                new CategoryDTO() {
                    Id = 4,
                    Name = "Groceries"
                },
                new CategoryDTO() {
                    Id = 5,
                    Name = "Hobby"
                },
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
