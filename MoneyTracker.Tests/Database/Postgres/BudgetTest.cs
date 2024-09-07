using MoneyTracker.Data.Postgres;
using MoneyTracker.DatabaseMigration;
using MoneyTracker.Shared.Models.Budget;
using MoneyTracker.Shared.Models.Transaction;
using MoneyTracker.Tests.Database.Postgres.TestModels;
using Newtonsoft.Json;
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
            var budget = new BudgetDatabase(db);

            var expected = new List<TestBudgetGroupDTO>()
            {
                new TestBudgetGroupDTO() {
                    Name = "Income",
                    Categories = [
                        new TestBudgetCategoryDTO() {
                            Name = "Wages & Salary : Net Pay",
                            Planned = 1800,
                            Actual = 1800,
                            Difference = 0
                        },
                    ],
                    Planned = 1800,
                    Actual = 1800,
                    Difference = 0
                },
                new TestBudgetGroupDTO() {
                    Name = "Committed Expenses",
                    Categories = [
                        new TestBudgetCategoryDTO() {
                            Name = "Bills : Cell Phone",
                            Planned = 10,
                            Actual = 10,
                            Difference = 0
                        },
                        new TestBudgetCategoryDTO() {
                            Name = "Bills : Rent",
                            Planned = 500,
                            Actual = 500,
                            Difference = 0
                        },
                        new TestBudgetCategoryDTO() {
                            Name = "Groceries",
                            Planned = 100,
                            Actual = 75,
                            Difference = 25
                        },
                    ],
                    Planned = 610,
                    Actual = 585,
                    Difference = 25
                },
                new TestBudgetGroupDTO() {
                    Name = "Fun",
                    Categories = [],
                    Planned = 0,
                    Actual = 0,
                    Difference = 0
                },
                new TestBudgetGroupDTO() {
                    Name = "Irregular Expenses",
                    Categories = [
                        new TestBudgetCategoryDTO() {
                            Name = "Hobby",
                            Planned = 50,
                            Actual = 150,
                            Difference = -100
                        }
                    ],
                    Planned = 50,
                    Actual = 150,
                    Difference = -100
                },
                new TestBudgetGroupDTO() {
                    Name = "Savings & Debt",
                    Categories = [],
                    Planned = 0,
                    Actual = 0,
                    Difference = 0
                },
                new TestBudgetGroupDTO() {
                    Name = "Retirement",
                    Categories = [],
                    Planned = 0,
                    Actual = 0,
                    Difference = 0
                },
            };
            var actual = await budget.GetBudget();
            for (int i = 0; i < actual.Count; i++)
            {
                var a = expected[i];
                var b = actual[i];
                if (!a.Equals(b))
                {
                    Assert.Fail($"{i}\n{JsonConvert.SerializeObject(expected[i])}\n{JsonConvert.SerializeObject(actual[i])}");
                }
            }
        }

        [Fact]
        public async void AddBudgetCategory_AddCategory_ReturnsBudgetWithNewCategory()
        {
            var db = new PostgresDatabase(_postgres.GetConnectionString());
            var budget = new BudgetDatabase(db);
            var newBudget = new NewBudgetCategoryDTO(2, 6, 180);
            await budget.AddBudgetCategory(newBudget);

            var budgetToBePutIntoExpected = new TestBudgetCategoryDTO()
            {
                Name = "Pet Care",
                Planned = 180,
                Actual = 0,
                Difference = 180,
            };

            var expected = new List<TestBudgetGroupDTO>()
            {
                new TestBudgetGroupDTO() {
                    Name = "Income",
                    Categories = [
                        new TestBudgetCategoryDTO() {
                            Name = "Wages & Salary : Net Pay",
                            Planned = 1800,
                            Actual = 1800,
                            Difference = 0
                        },
                    ],
                    Planned = 1800,
                    Actual = 1800,
                    Difference = 0
                },
                new TestBudgetGroupDTO() {
                    Name = "Committed Expenses",
                    Categories = [
                        new TestBudgetCategoryDTO() {
                            Name = "Bills : Cell Phone",
                            Planned = 10,
                            Actual = 10,
                            Difference = 0
                        },
                        new TestBudgetCategoryDTO() {
                            Name = "Bills : Rent",
                            Planned = 500,
                            Actual = 500,
                            Difference = 0
                        },
                        new TestBudgetCategoryDTO() {
                            Name = "Groceries",
                            Planned = 100,
                            Actual = 75,
                            Difference = 25
                        },
                        budgetToBePutIntoExpected,
                    ],
                    Planned = 610 + budgetToBePutIntoExpected.Planned,
                    Actual = 585 + budgetToBePutIntoExpected.Actual,
                    Difference = 25 + budgetToBePutIntoExpected.Difference,
                },
                new TestBudgetGroupDTO() {
                    Name = "Fun",
                    Categories = [],
                    Planned = 0,
                    Actual = 0,
                    Difference = 0
                },
                new TestBudgetGroupDTO() {
                    Name = "Irregular Expenses",
                    Categories = [
                        new TestBudgetCategoryDTO() {
                            Name = "Hobby",
                            Planned = 50,
                            Actual = 150,
                            Difference = -100
                        }
                    ],
                    Planned = 50,
                    Actual = 150,
                    Difference = -100
                },
                new TestBudgetGroupDTO() {
                    Name = "Savings & Debt",
                    Categories = [],
                    Planned = 0,
                    Actual = 0,
                    Difference = 0
                },
                new TestBudgetGroupDTO() {
                    Name = "Retirement",
                    Categories = [],
                    Planned = 0,
                    Actual = 0,
                    Difference = 0
                },
            };
            var actual = await budget.GetBudget();
            for (int i = 0; i < actual.Count; i++)
            {
                var a = expected[i];
                var b = actual[i];
                if (!a.Equals(b))
                {
                    Assert.Fail($"{i}\n{JsonConvert.SerializeObject(expected[i])}\n{JsonConvert.SerializeObject(actual[i])}");
                }
            }
        }

        [Fact]
        public async void GetBudget_AddBillUpdatesBudgetInCategory_ReturnsSameBudgetButWithActualUpdated()
        {
            var db = new PostgresDatabase(_postgres.GetConnectionString());
            var transaction = new RegisterDatabase(db);
            await transaction.AddNewTransaction(new NewTransactionDTO("bob", 17, new DateTime(2024, 08, 29), 4));

            var budget = new BudgetDatabase(db);

            var expected = new List<TestBudgetGroupDTO>()
            {
                new TestBudgetGroupDTO() {
                    Name = "Income",
                    Categories = [
                        new TestBudgetCategoryDTO() {
                            Name = "Wages & Salary : Net Pay",
                            Planned = 1800,
                            Actual = 1800,
                            Difference = 0
                        },
                    ],
                    Planned = 1800,
                    Actual = 1800,
                    Difference = 0
                },
                new TestBudgetGroupDTO() {
                    Name = "Committed Expenses",
                    Categories = [
                        new TestBudgetCategoryDTO() {
                            Name = "Bills : Cell Phone",
                            Planned = 10,
                            Actual = 10,
                            Difference = 0
                        },
                        new TestBudgetCategoryDTO() {
                            Name = "Bills : Rent",
                            Planned = 500,
                            Actual = 500,
                            Difference = 0
                        },
                        new TestBudgetCategoryDTO() {
                            Name = "Groceries",
                            Planned = 100,
                            Actual = 92,
                            Difference = 8
                        },
                    ],
                    Planned = 610,
                    Actual = 602,
                    Difference = 8,
                },
                new TestBudgetGroupDTO() {
                    Name = "Fun",
                    Categories = [],
                    Planned = 0,
                    Actual = 0,
                    Difference = 0
                },
                new TestBudgetGroupDTO() {
                    Name = "Irregular Expenses",
                    Categories = [
                        new TestBudgetCategoryDTO() {
                            Name = "Hobby",
                            Planned = 50,
                            Actual = 150,
                            Difference = -100
                        }
                    ],
                    Planned = 50,
                    Actual = 150,
                    Difference = -100
                },
                new TestBudgetGroupDTO() {
                    Name = "Savings & Debt",
                    Categories = [],
                    Planned = 0,
                    Actual = 0,
                    Difference = 0
                },
                new TestBudgetGroupDTO() {
                    Name = "Retirement",
                    Categories = [],
                    Planned = 0,
                    Actual = 0,
                    Difference = 0
                },
            };
            var actual = await budget.GetBudget();
            for (int i = 0; i < actual.Count; i++)
            {
                var a = expected[i];
                var b = actual[i];
                if (!a.Equals(b))
                {
                    Assert.Fail($"{i}\n{JsonConvert.SerializeObject(expected[i])}\n{JsonConvert.SerializeObject(actual[i])}");
                }
            }
        }

        [Fact]
        public async void AddBudgetCategory_AddDuplicateCategory_UpdatesCategory()
        {
            var db = new PostgresDatabase(_postgres.GetConnectionString());
            var budget = new BudgetDatabase(db);
            var newBudget = new NewBudgetCategoryDTO(2, 4, 150);
            await budget.AddBudgetCategory(newBudget);

            var expected = new List<TestBudgetGroupDTO>()
            {
                new TestBudgetGroupDTO() {
                    Name = "Income",
                    Categories = [
                        new TestBudgetCategoryDTO() {
                            Name = "Wages & Salary : Net Pay",
                            Planned = 1800,
                            Actual = 1800,
                            Difference = 0
                        },
                    ],
                    Planned = 1800,
                    Actual = 1800,
                    Difference = 0
                },
                new TestBudgetGroupDTO() {
                    Name = "Committed Expenses",
                    Categories = [
                        new TestBudgetCategoryDTO() {
                            Name = "Bills : Cell Phone",
                            Planned = 10,
                            Actual = 10,
                            Difference = 0
                        },
                        new TestBudgetCategoryDTO() {
                            Name = "Bills : Rent",
                            Planned = 500,
                            Actual = 500,
                            Difference = 0
                        },
                        new TestBudgetCategoryDTO() {
                            Name = "Groceries",
                            Planned = 150,
                            Actual = 75,
                            Difference = 75
                        },
                    ],
                    Planned = 660,
                    Actual = 585,
                    Difference = 75,
                },
                new TestBudgetGroupDTO() {
                    Name = "Fun",
                    Categories = [],
                    Planned = 0,
                    Actual = 0,
                    Difference = 0
                },
                new TestBudgetGroupDTO() {
                    Name = "Irregular Expenses",
                    Categories = [
                        new TestBudgetCategoryDTO() {
                            Name = "Hobby",
                            Planned = 50,
                            Actual = 150,
                            Difference = -100
                        }
                    ],
                    Planned = 50,
                    Actual = 150,
                    Difference = -100
                },
                new TestBudgetGroupDTO() {
                    Name = "Savings & Debt",
                    Categories = [],
                    Planned = 0,
                    Actual = 0,
                    Difference = 0
                },
                new TestBudgetGroupDTO() {
                    Name = "Retirement",
                    Categories = [],
                    Planned = 0,
                    Actual = 0,
                    Difference = 0
                },
            };
            var actual = await budget.GetBudget();
            for (int i = 0; i < actual.Count; i++)
            {
                var a = expected[i];
                var b = actual[i];
                if (!a.Equals(b))
                {
                    Assert.Fail($"{i}\n{JsonConvert.SerializeObject(expected[i])}\n{JsonConvert.SerializeObject(actual[i])}");
                }
            }
        }

        [Fact]
        public async void EditBudgetCategory_ChangePlanned_UpdatesPlanned()
        {
            var db = new PostgresDatabase(_postgres.GetConnectionString());
            var budget = new BudgetDatabase(db);
            var newBudget = new EditBudgetCategoryDTO(4, budgetCategoryPlanned: 150);
            await budget.EditBudgetCategory(newBudget);

            var expected = new List<TestBudgetGroupDTO>()
            {
                new TestBudgetGroupDTO() {
                    Name = "Income",
                    Categories = [
                        new TestBudgetCategoryDTO() {
                            Name = "Wages & Salary : Net Pay",
                            Planned = 1800,
                            Actual = 1800,
                            Difference = 0
                        },
                    ],
                    Planned = 1800,
                    Actual = 1800,
                    Difference = 0
                },
                new TestBudgetGroupDTO() {
                    Name = "Committed Expenses",
                    Categories = [
                        new TestBudgetCategoryDTO() {
                            Name = "Bills : Cell Phone",
                            Planned = 10,
                            Actual = 10,
                            Difference = 0
                        },
                        new TestBudgetCategoryDTO() {
                            Name = "Bills : Rent",
                            Planned = 500,
                            Actual = 500,
                            Difference = 0
                        },
                        new TestBudgetCategoryDTO() {
                            Name = "Groceries",
                            Planned = 150,
                            Actual = 75,
                            Difference = 75
                        },
                    ],
                    Planned = 660,
                    Actual = 585,
                    Difference = 75,
                },
                new TestBudgetGroupDTO() {
                    Name = "Fun",
                    Categories = [],
                    Planned = 0,
                    Actual = 0,
                    Difference = 0
                },
                new TestBudgetGroupDTO() {
                    Name = "Irregular Expenses",
                    Categories = [
                        new TestBudgetCategoryDTO() {
                            Name = "Hobby",
                            Planned = 50,
                            Actual = 150,
                            Difference = -100
                        }
                    ],
                    Planned = 50,
                    Actual = 150,
                    Difference = -100
                },
                new TestBudgetGroupDTO() {
                    Name = "Savings & Debt",
                    Categories = [],
                    Planned = 0,
                    Actual = 0,
                    Difference = 0
                },
                new TestBudgetGroupDTO() {
                    Name = "Retirement",
                    Categories = [],
                    Planned = 0,
                    Actual = 0,
                    Difference = 0
                },
            };
            var actual = await budget.GetBudget();
            for (int i = 0; i < actual.Count; i++)
            {
                var a = expected[i];
                var b = actual[i];
                if (!a.Equals(b))
                {
                    Assert.Fail($"{i}\n{JsonConvert.SerializeObject(expected[i])}\n{JsonConvert.SerializeObject(actual[i])}");
                }
            }
        }
    }
}
