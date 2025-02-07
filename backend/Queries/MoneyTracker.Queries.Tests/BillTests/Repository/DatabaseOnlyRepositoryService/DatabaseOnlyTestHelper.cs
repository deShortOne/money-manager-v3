using MoneyTracker.Queries.Domain.Repositories.Database;
using MoneyTracker.Queries.Infrastructure.Service.DatabaseOnly;
using Moq;

namespace MoneyTracker.Queries.Tests.BillTests.Repository.DatabaseOnlyRepositoryService;
public class DatabaseOnlyTestHelper
{
    protected Mock<IBillDatabase> _mockBillDatabase;

    protected BillRepository _billRepositoryService;

    protected DatabaseOnlyTestHelper()
    {
        _mockBillDatabase = new Mock<IBillDatabase>();

        _billRepositoryService = new BillRepository(
            _mockBillDatabase.Object
        );
    }

    protected void VerifyNoOtherCalls()
    {
        _mockBillDatabase.VerifyNoOtherCalls();
    }
}
