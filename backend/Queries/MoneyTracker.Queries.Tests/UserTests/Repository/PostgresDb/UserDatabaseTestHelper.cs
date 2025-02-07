using MoneyTracker.Common.Utilities.DateTimeUtil;
using MoneyTracker.Queries.Domain.Repositories.Database;
using MoneyTracker.Queries.Infrastructure.Postgres;
using MoneyTracker.Queries.Tests.Fixture;
using Moq;

namespace MoneyTracker.Queries.Tests.UserTests.Repository.PostgresDb;
public class UserDatabaseTestHelper : IClassFixture<PostgresDbFixture>
{
    protected readonly IUserDatabase _userRepository;
    protected readonly Mock<IDateTimeProvider> _mockDateTimeProvider;

    public UserDatabaseTestHelper(PostgresDbFixture postgresFixture)
    {
        _mockDateTimeProvider = new Mock<IDateTimeProvider>();
        var database = new PostgresDatabase(postgresFixture.ConnectionString);

        _userRepository = new UserDatabase(database, _mockDateTimeProvider.Object);
    }
}
