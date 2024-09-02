using DatabaseMigration;
using MoneyTracker.Data.Postgres;
using MoneyTracker.Shared.Models.Budget;
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
            var db = new Helper(_postgres.GetConnectionString());
            var budget = new Budget(db);

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
                        }
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
                            Name = "Groceries",
                            Planned = 100,
                            Actual = 75,
                            Difference = 25
                            },
                        new TestBudgetCategoryDTO() {
                            Name = "Bills : Rent",
                            Planned = 500,
                            Actual = 500,
                            Difference = 0
                            }
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
                if (a.Planned != b.Planned || a.Actual != b.Actual || a.Difference != b.Difference)
                {
                    Assert.Fail($"{i}\n{JsonConvert.SerializeObject(expected[i])}\n{JsonConvert.SerializeObject(actual[i])}");
                }
            }
        }

        private static bool CompareLists(List<TestBudgetCategoryDTO> expected, List<BudgetCategoryDTO> actual)
        {

            if (expected.Count != actual.Count)
            {
                return false;
            }
            for (int i = 0; i < expected.Count; i++)
            {
                if (expected[i].Actual != actual[i].Actual ||
                    expected[i].Name != actual[i].Name ||
                    expected[i].Planned != actual[i].Planned ||
                    expected[i].Difference != actual[i].Difference)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
