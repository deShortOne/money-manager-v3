//using MoneyTracker.Data.Postgres;
//using MoneyTracker.DatabaseMigration;
//using MoneyTracker.DatabaseMigration.Models;
//using MoneyTracker.Shared.Models.RepositoryToService.Budget;
//using MoneyTracker.Shared.Models.ServiceToRepository.Budget;
//using MoneyTracker.Shared.Models.ServiceToRepository.Transaction;
//using Testcontainers.PostgreSql;

//namespace MoneyTracker.Tests.OldTestsToMoveOver.Database.Postgres
//{
//    public sealed class BudgetTest : IAsyncLifetime
//    {
//        private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
//#if RUN_LOCAL
//            .WithDockerEndpoint("tcp://localhost:2375")
//#endif
//            .WithImage("postgres:16")
//            .WithCleanUp(true)
//            .Build();

//        public async Task InitializeAsync()
//        {
//            await _postgres.StartAsync();

//            Migration.CheckMigration(_postgres.GetConnectionString(), new MigrationOption(true, true));

//            return;
//        }

//        public Task DisposeAsync()
//        {
//            return _postgres.DisposeAsync().AsTask();
//        }

//        [Fact]
//        public async void FirstLoadCheckTablesThatDataAreThere()
//        {
//            var db = new PostgresDatabase(_postgres.GetConnectionString());
//            var budget = new BudgetRepository(db);

//            var expected = new List<BudgetGroupEntityDTO>()
//            {
//                new BudgetGroupEntityDTO("Income", 1800, 1800, 0, [
//                        new BudgetCategoryEntityDTO("Wages & Salary : Net Pay", 1800, 1800, 0),
//                    ]
//                ),
//                new BudgetGroupEntityDTO("Committed Expenses", 610, 585, 25, [
//                        new BudgetCategoryEntityDTO("Bills : Cell Phone", 10, 10, 0),
//                        new BudgetCategoryEntityDTO("Bills : Rent", 500, 500, 0),
//                        new BudgetCategoryEntityDTO("Groceries", 100, 75, 25),
//                    ]
//                ),
//                new BudgetGroupEntityDTO("Fun", 0, 0, 0, []),
//                new BudgetGroupEntityDTO("Irregular Expenses", 50, 150, -100, [
//                        new BudgetCategoryEntityDTO("Hobby", 50, 150, -100),
//                    ]
//                ),
//                new BudgetGroupEntityDTO("Savings & Debt", 0, 0, 0, []),
//                new BudgetGroupEntityDTO("Retirement", 0, 0, 0, []),
//            };

//            var actual = await budget.GetBudget();
//            Assert.Equal(expected, actual);
//        }

//        [Fact]
//        public async void AddBudgetCategory_AddCategory_ReturnsBudgetWithNewCategory()
//        {
//            var db = new PostgresDatabase(_postgres.GetConnectionString());
//            var budget = new BudgetRepository(db);
//            var newBudget = new NewBudgetCategoryDTO(2, 6, 180);
//            await budget.AddBudgetCategory(newBudget);

//            var budgetToBePutIntoExpected = new BudgetCategoryEntityDTO("Pet Care", 180, 0, 180);

//            var expected = new List<BudgetGroupEntityDTO>()
//            {
//                new BudgetGroupEntityDTO("Income", 1800, 1800, 0, [
//                        new BudgetCategoryEntityDTO("Wages & Salary : Net Pay", 1800, 1800, 0),
//                    ]
//                ),
//                new BudgetGroupEntityDTO("Committed Expenses",
//                    610 + budgetToBePutIntoExpected.Planned,
//                    585 + budgetToBePutIntoExpected.Actual,
//                    25 + budgetToBePutIntoExpected.Difference,
//                    [
//                        new BudgetCategoryEntityDTO("Bills : Cell Phone", 10, 10, 0),
//                        new BudgetCategoryEntityDTO("Bills : Rent", 500, 500, 0),
//                        new BudgetCategoryEntityDTO("Groceries", 100, 75, 25),
//                        budgetToBePutIntoExpected,
//                    ]
//                ),
//                new BudgetGroupEntityDTO("Fun", 0, 0, 0, []),
//                new BudgetGroupEntityDTO("Irregular Expenses", 50, 150, -100, [
//                        new BudgetCategoryEntityDTO("Hobby", 50, 150, -100),
//                    ]
//                ),
//                new BudgetGroupEntityDTO("Savings & Debt", 0, 0, 0, []),
//                new BudgetGroupEntityDTO("Retirement", 0, 0, 0, []),
//            };

//            var actual = await budget.GetBudget();
//            Assert.Equal(expected, actual);
//        }

//        [Fact]
//        public async void GetBudget_AddBillUpdatesBudgetInCategory_ReturnsSameBudgetButWithActualUpdated()
//        {
//            var db = new PostgresDatabase(_postgres.GetConnectionString());
//            var transaction = new RegisterRepository(db);
//            await transaction.AddTransaction(new NewTransactionDTO("bob", 17, new DateOnly(2024, 08, 29), 4, 1));

//            var budget = new BudgetRepository(db);

//            var expected = new List<BudgetGroupEntityDTO>()
//            {
//                new BudgetGroupEntityDTO("Income", 1800, 1800, 0, [
//                        new BudgetCategoryEntityDTO("Wages & Salary : Net Pay", 1800, 1800, 0),
//                    ]
//                ),
//                new BudgetGroupEntityDTO("Committed Expenses", 610, 602, 8, [ // EDITED
//                        new BudgetCategoryEntityDTO("Bills : Cell Phone", 10, 10, 0),
//                        new BudgetCategoryEntityDTO("Bills : Rent", 500, 500, 0),
//                        new BudgetCategoryEntityDTO("Groceries", 100, 92, 8),// EDITED
//                    ]
//                ),
//                new BudgetGroupEntityDTO("Fun", 0, 0, 0, []),
//                new BudgetGroupEntityDTO("Irregular Expenses", 50, 150, -100, [
//                        new BudgetCategoryEntityDTO("Hobby", 50, 150, -100),
//                    ]
//                ),
//                new BudgetGroupEntityDTO("Savings & Debt", 0, 0, 0, []),
//                new BudgetGroupEntityDTO("Retirement", 0, 0, 0, []),
//            };

//            var actual = await budget.GetBudget();
//            Assert.Equal(expected, actual);
//        }

//        [Fact]
//        public async void AddBudgetCategory_AddDuplicateCategory_UpdatesCategory()
//        {
//            var db = new PostgresDatabase(_postgres.GetConnectionString());
//            var budget = new BudgetRepository(db);
//            var newBudget = new NewBudgetCategoryDTO(2, 4, 150);
//            await budget.AddBudgetCategory(newBudget);

//            var expected = new List<BudgetGroupEntityDTO>()
//            {
//                new BudgetGroupEntityDTO("Income", 1800, 1800, 0, [
//                        new BudgetCategoryEntityDTO("Wages & Salary : Net Pay", 1800, 1800, 0),
//                    ]
//                ),
//                new BudgetGroupEntityDTO("Committed Expenses", 660, 585, 75, [
//                        new BudgetCategoryEntityDTO("Bills : Cell Phone", 10, 10, 0),
//                        new BudgetCategoryEntityDTO("Bills : Rent", 500, 500, 0),
//                        new BudgetCategoryEntityDTO("Groceries", 150, 75, 75),
//                    ]
//                ),
//                new BudgetGroupEntityDTO("Fun", 0, 0, 0, []),
//                new BudgetGroupEntityDTO("Irregular Expenses", 50, 150, -100, [
//                        new BudgetCategoryEntityDTO("Hobby", 50, 150, -100),
//                    ]
//                ),
//                new BudgetGroupEntityDTO("Savings & Debt", 0, 0, 0, []),
//                new BudgetGroupEntityDTO("Retirement", 0, 0, 0, []),
//            };

//            var actual = await budget.GetBudget();
//            Assert.Equal(expected, actual);
//        }

//        [Fact]
//        public async void EditBudgetCategory_ChangePlanned_UpdatesPlanned()
//        {
//            var db = new PostgresDatabase(_postgres.GetConnectionString());
//            var budget = new BudgetRepository(db);
//            var newBudget = new EditBudgetCategoryDTO(4, budgetCategoryPlanned: 150);
//            await budget.EditBudgetCategory(newBudget);

//            var expected = new List<BudgetGroupEntityDTO>()
//            {
//                new BudgetGroupEntityDTO("Income", 1800, 1800, 0, [
//                        new BudgetCategoryEntityDTO("Wages & Salary : Net Pay", 1800, 1800, 0),
//                    ]
//                ),
//                new BudgetGroupEntityDTO("Committed Expenses", 660, 585, 75, [
//                        new BudgetCategoryEntityDTO("Bills : Cell Phone", 10, 10, 0),
//                        new BudgetCategoryEntityDTO("Bills : Rent", 500, 500, 0),
//                        new BudgetCategoryEntityDTO("Groceries", 150, 75, 75),
//                    ]
//                ),
//                new BudgetGroupEntityDTO("Fun", 0, 0, 0, []),
//                new BudgetGroupEntityDTO("Irregular Expenses", 50, 150, -100, [
//                        new BudgetCategoryEntityDTO("Hobby", 50, 150, -100),
//                    ]
//                ),
//                new BudgetGroupEntityDTO("Savings & Debt", 0, 0, 0, []),
//                new BudgetGroupEntityDTO("Retirement", 0, 0, 0, []),
//            };

//            var actual = await budget.GetBudget();
//            Assert.Equal(expected, actual);
//        }
//    }
//}
