
using MoneyTracker.Shared.DateManager;
using Moq;

namespace MoneyTracker.Tests.Local;
internal class TestHelper
{
    public static IDateTimeProvider CreateMockDateTimeProvider(DateTime dateTime)
    {
        var mockDateTime = new Mock<IDateTimeProvider>();
        mockDateTime.Setup(dateTimeTmp => dateTimeTmp.Now).Returns(dateTime);

        return mockDateTime.Object;
    }
}
