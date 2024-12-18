
namespace MoneyTracker.Tests.FrequencyCalculationTests.MonthDayCalculator;
public sealed class Calculate
{
    [Theory]
    [InlineData(2024, 8, 31)]
    [InlineData(2024, 6, 30)]
    [InlineData(2024, 2, 29)]
    [InlineData(2023, 2, 28)]
    public void LastDayOfMonth_Returns31(int year, int month, int day)
    {
        Assert.Equal(31,
            new Common.Utilities.CalculationUtil.MonthDayCalculator().Calculate(
                new DateOnly(year, month, day)));
    }

    [Theory]
    [InlineData(2024, 8, 30)]
    [InlineData(2024, 6, 29)]
    [InlineData(2024, 2, 28)]
    [InlineData(2023, 2, 27)]
    [InlineData(2023, 11, 2)]
    public void NotLastDayOfMonth_ReturnsThatDay(int year, int month, int day)
    {
        Assert.Equal(day,
            new Common.Utilities.CalculationUtil.MonthDayCalculator().Calculate(
                new DateOnly(year, month, day)));
    }
}
