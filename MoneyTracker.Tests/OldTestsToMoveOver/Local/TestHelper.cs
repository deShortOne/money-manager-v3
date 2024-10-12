using MoneyTracker.Shared.DateManager;
using Moq;

namespace MoneyTracker.Tests.OldTestsToMoveOver.Local;
internal class TestHelper
{
    public static IDateProvider CreateMockdateProvider(DateOnly dateTime)
    {
        var mockDateTime = new Mock<IDateProvider>();
        mockDateTime.Setup(dateTimeTmp => dateTimeTmp.Now).Returns(dateTime);

        return mockDateTime.Object;
    }
}
