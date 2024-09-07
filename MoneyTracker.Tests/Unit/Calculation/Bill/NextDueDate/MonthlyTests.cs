
using MoneyTracker.Calculation.Bill.Frequencies;
using MoneyTracker.Shared.DateManager;
using MoneyTracker.Shared.Models.Bill;
using MoneyTracker.Tests.Local;

namespace MoneyTracker.Tests.Unit.Calculation.Bill.NextDueDate;
public sealed class MonthlyTests
{
    [Fact]
    public void GetNextDueDate()
    {
        var month = new Monthly();

        var nextDueDate = month.CalculateNextDueDate(24, new DateOnly(2024, 8, 24));
        Assert.Equal(new DateOnly(2024, 9, 24), nextDueDate);
    }

    [Fact]
    public void GetNextDueDate_31stTo31st()
    {
        var month = new Monthly();

        var nextDueDate = month.CalculateNextDueDate(31, new DateOnly(2024, 7, 31));
        Assert.Equal(new DateOnly(2024, 8, 31), nextDueDate);
    }

    [Fact]
    public void GetNextDueDate_31stTo30th()
    {
        var month = new Monthly();

        var nextDueDate = month.CalculateNextDueDate(31, new DateOnly(2024, 5, 31));
        Assert.Equal(new DateOnly(2024, 6, 30), nextDueDate);
    }

    [Fact]
    public void GetNextDueDate_30thTo31st()
    {
        var month = new Monthly();

        var nextDueDate = month.CalculateNextDueDate(31, new DateOnly(2024, 6, 30));
        Assert.Equal(new DateOnly(2024, 7, 31), nextDueDate);
    }

    [Fact]
    public void GetNextDueDate_30thTo30th()
    {
        var month = new Monthly();

        var nextDueDate = month.CalculateNextDueDate(30, new DateOnly(2024, 7, 30));
        Assert.Equal(new DateOnly(2024, 8, 30), nextDueDate);
    }
}
