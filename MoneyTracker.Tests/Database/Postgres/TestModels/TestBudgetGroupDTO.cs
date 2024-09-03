
namespace MoneyTracker.Tests.Database.Postgres.TestModels;

public class TestBudgetGroupDTO
{
    public string Name { get; set; }
    public List<TestBudgetCategoryDTO> Categories { get; set; } = [];
    public decimal Planned { get; set; }
    public decimal Actual { get; set; }
    public decimal Difference { get; set; }
}
