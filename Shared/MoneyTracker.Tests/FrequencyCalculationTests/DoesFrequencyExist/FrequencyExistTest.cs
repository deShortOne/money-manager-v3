using MoneyTracker.Common.Utilities.CalculationUtil;

namespace MoneyTracker.Tests.FrequencyCalculationTests.DoesFrequencyExist;
public sealed class FrequencyExistTest
{
    [Fact]
    public void DailyExist()
    {
        var frequency = new FrequencyCalculation();
        Assert.True(frequency.DoesFrequencyExist("Daily"));
    }
    [Fact]
    public void WeeklyExist()
    {
        var frequency = new FrequencyCalculation();
        Assert.True(frequency.DoesFrequencyExist("Weekly"));
    }
    [Fact]
    public void BiWeeklyExist()
    {
        var frequency = new FrequencyCalculation();
        Assert.True(frequency.DoesFrequencyExist("BiWeekly"));
    }
    [Fact]
    public void MonthlyExist()
    {
        var frequency = new FrequencyCalculation();
        Assert.True(frequency.DoesFrequencyExist("Monthly"));
    }
    [Fact]
    public void DoesNotExist()
    {
        var frequency = new FrequencyCalculation();
        Assert.False(frequency.DoesFrequencyExist("1.2khz"));
    }
}
