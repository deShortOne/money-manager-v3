using MoneyTracker.Data.Postgres;
using MoneyTracker.DatabaseMigration;
using MoneyTracker.DatabaseMigration.Models;
using MoneyTracker.Shared.Models.Budget;
using MoneyTracker.Shared.Models.Transaction;
using Testcontainers.PostgreSql;

namespace MoneyTracker.Tests.Database.Postgres
{
    public sealed class BudgetTest : IAsyncLifetime
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

            Migration.CheckMigration(_postgres.GetConnectionString(), new MigrationOption(true));

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
            var budget = new BudgetDatabase(db);

            var expected = new List<BudgetGroupDTO>()
            {
                new BudgetGroupDTO("Income", 1800, 1800, 0, [
                        new BudgetCategoryDTO("Wages & Salary : Net Pay", 1800, 1800, 0),
                    ]
                ),
                new BudgetGroupDTO("Committed Expenses", 610, 585, 25, [
                        new BudgetCategoryDTO("Bills : Cell Phone", 10, 10, 0),
                        new BudgetCategoryDTO("Bills : Rent", 500, 500, 0),
                        new BudgetCategoryDTO("Groceries", 100, 75, 25),
                    ]
                ),
                new BudgetGroupDTO("Fun", 0, 0, 0, []),
                new BudgetGroupDTO("Irregular Expenses", 50, 150, -100, [
                        new BudgetCategoryDTO("Hobby", 50, 150, -100),
                    ]
                ),
                new BudgetGroupDTO("Savings & Debt", 0, 0, 0, []),
                new BudgetGroupDTO("Retirement", 0, 0, 0, []),
            };

            var actual = await budget.GetBudget();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async void AddBudgetCategory_AddCategory_ReturnsBudgetWithNewCategory()
        {
            var db = new PostgresDatabase(_postgres.GetConnectionString());
            var budget = new BudgetDatabase(db);
            var newBudget = new NewBudgetCategoryDTO(2, 6, 180);
            await budget.AddBudgetCategory(newBudget);

            var budgetToBePutIntoExpected = new BudgetCategoryDTO("Pet Care", 180, 0, 180);

            var expected = new List<BudgetGroupDTO>()
            {
                new BudgetGroupDTO("Income", 1800, 1800, 0, [
                        new BudgetCategoryDTO("Wages & Salary : Net Pay", 1800, 1800, 0),
                    ]
                ),
                new BudgetGroupDTO("Committed Expenses",
                    610 + budgetToBePutIntoExpected.Planned,
                    585 + budgetToBePutIntoExpected.Actual,
                    25 + budgetToBePutIntoExpected.Difference,
                    [
                        new BudgetCategoryDTO("Bills : Cell Phone", 10, 10, 0),
                        new BudgetCategoryDTO("Bills : Rent", 500, 500, 0),
                        new BudgetCategoryDTO("Groceries", 100, 75, 25),
                        budgetToBePutIntoExpected,
                    ]
                ),
                new BudgetGroupDTO("Fun", 0, 0, 0, []),
                new BudgetGroupDTO("Irregular Expenses", 50, 150, -100, [
                        new BudgetCategoryDTO("Hobby", 50, 150, -100),
                    ]
                ),
                new BudgetGroupDTO("Savings & Debt", 0, 0, 0, []),
                new BudgetGroupDTO("Retirement", 0, 0, 0, []),
            };

            var actual = await budget.GetBudget();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async void GetBudget_AddBillUpdatesBudgetInCategory_ReturnsSameBudgetButWithActualUpdated()
        {
            var db = new PostgresDatabase(_postgres.GetConnectionString());
            var transaction = new RegisterDatabase(db);
            await transaction.AddTransaction(new NewTransactionDTO("bob", 17, new DateTime(2024, 08, 29), 4));

            var budget = new BudgetDatabase(db);

            var expected = new List<BudgetGroupDTO>()
            {
                new BudgetGroupDTO("Income", 1800, 1800, 0, [
                        new BudgetCategoryDTO("Wages & Salary : Net Pay", 1800, 1800, 0),
                    ]
                ),
                new BudgetGroupDTO("Committed Expenses", 610, 602, 8, [ // EDITED
                        new BudgetCategoryDTO("Bills : Cell Phone", 10, 10, 0),
                        new BudgetCategoryDTO("Bills : Rent", 500, 500, 0),
                        new BudgetCategoryDTO("Groceries", 100, 92, 8),// EDITED
                    ]
                ),
                new BudgetGroupDTO("Fun", 0, 0, 0, []),
                new BudgetGroupDTO("Irregular Expenses", 50, 150, -100, [
                        new BudgetCategoryDTO("Hobby", 50, 150, -100),
                    ]
                ),
                new BudgetGroupDTO("Savings & Debt", 0, 0, 0, []),
                new BudgetGroupDTO("Retirement", 0, 0, 0, []),
            };

            var actual = await budget.GetBudget();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async void AddBudgetCategory_AddDuplicateCategory_UpdatesCategory()
        {
            var db = new PostgresDatabase(_postgres.GetConnectionString());
            var budget = new BudgetDatabase(db);
            var newBudget = new NewBudgetCategoryDTO(2, 4, 150);
            await budget.AddBudgetCategory(newBudget);

            var expected = new List<BudgetGroupDTO>()
            {
                new BudgetGroupDTO("Income", 1800, 1800, 0, [
                        new BudgetCategoryDTO("Wages & Salary : Net Pay", 1800, 1800, 0),
                    ]
                ),
                new BudgetGroupDTO("Committed Expenses", 660, 585, 75, [
                        new BudgetCategoryDTO("Bills : Cell Phone", 10, 10, 0),
                        new BudgetCategoryDTO("Bills : Rent", 500, 500, 0),
                        new BudgetCategoryDTO("Groceries", 150, 75, 75),
                    ]
                ),
                new BudgetGroupDTO("Fun", 0, 0, 0, []),
                new BudgetGroupDTO("Irregular Expenses", 50, 150, -100, [
                        new BudgetCategoryDTO("Hobby", 50, 150, -100),
                    ]
                ),
                new BudgetGroupDTO("Savings & Debt", 0, 0, 0, []),
                new BudgetGroupDTO("Retirement", 0, 0, 0, []),
            };

            var actual = await budget.GetBudget();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async void EditBudgetCategory_ChangePlanned_UpdatesPlanned()
        {
            var db = new PostgresDatabase(_postgres.GetConnectionString());
            var budget = new BudgetDatabase(db);
            var newBudget = new EditBudgetCategoryDTO(4, budgetCategoryPlanned: 150);
            await budget.EditBudgetCategory(newBudget);

            var expected = new List<BudgetGroupDTO>()
            {
                new BudgetGroupDTO("Income", 1800, 1800, 0, [
                        new BudgetCategoryDTO("Wages & Salary : Net Pay", 1800, 1800, 0),
                    ]
                ),
                new BudgetGroupDTO("Committed Expenses", 660, 585, 75, [
                        new BudgetCategoryDTO("Bills : Cell Phone", 10, 10, 0),
                        new BudgetCategoryDTO("Bills : Rent", 500, 500, 0),
                        new BudgetCategoryDTO("Groceries", 150, 75, 75),
                    ]
                ),
                new BudgetGroupDTO("Fun", 0, 0, 0, []),
                new BudgetGroupDTO("Irregular Expenses", 50, 150, -100, [
                        new BudgetCategoryDTO("Hobby", 50, 150, -100),
                    ]
                ),
                new BudgetGroupDTO("Savings & Debt", 0, 0, 0, []),
                new BudgetGroupDTO("Retirement", 0, 0, 0, []),
            };

            var actual = await budget.GetBudget();
            Assert.Equal(expected, actual);
        }
    }
}
