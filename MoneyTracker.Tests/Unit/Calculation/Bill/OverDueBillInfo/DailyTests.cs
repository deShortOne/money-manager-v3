
using MoneyTracker.Calculation.Bill.Frequencies;
using MoneyTracker.Shared.DateManager;
using Moq;

namespace MoneyTracker.Tests.Unit.Calculation.Bill.OverDueBillInfo;
public sealed class DailyTests
{
    [Fact]
    public void CalculateOverDueBillInfo_SameDay_Null()
    {
        var mockDateTime = new Mock<IDateTimeProvider>();
        mockDateTime.Setup(dateTime => dateTime.Now).Returns(new DateTime(2024, 8, 24, 0, 0, 0));

        IDateTimeProvider dateTimeProvider = mockDateTime.Object;

        var day = new Daily();

        Assert.Null(day.Calculate(new DateOnly(2024, 8, 24), dateTimeProvider));
    }

    [Fact]
    public void CalculateOverDueBillInfo_CurrentDayBeforeNextDueDate_Null()
    {
        var mockDateTime = new Mock<IDateTimeProvider>();
        mockDateTime.Setup(dateTime => dateTime.Now).Returns(new DateTime(2024, 8, 24, 0, 0, 0));

        IDateTimeProvider dateTimeProvider = mockDateTime.Object;

        var day = new Daily();

        Assert.Null(day.Calculate(new DateOnly(2024, 8, 24), dateTimeProvider));
    }
}
