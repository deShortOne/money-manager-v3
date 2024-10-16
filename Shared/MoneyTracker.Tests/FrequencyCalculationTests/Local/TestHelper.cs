using MoneyTracker.Common.Utilities.DateTimeUtil;
using Moq;

namespace MoneyTracker.Tests.FrequencyCalculationTests.Local;
internal class TestHelper
{
    public static IDateTimeProvider CreateMockdateProvider(DateTime dateTime)
    {
        var mockDateTime = new Mock<IDateTimeProvider>();
        mockDateTime.Setup(dateTimeTmp => dateTimeTmp.Now).Returns(dateTime);

        return mockDateTime.Object;
    }
}
