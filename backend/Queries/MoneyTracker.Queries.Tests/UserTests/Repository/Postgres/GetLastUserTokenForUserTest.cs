using MoneyTracker.Authentication.Entities;
using MoneyTracker.Common.Utilities.DateTimeUtil;
using MoneyTracker.Queries.DatabaseMigration;
using MoneyTracker.Queries.DatabaseMigration.Models;
using MoneyTracker.Queries.Domain.Repositories.Database;
using MoneyTracker.Queries.Infrastructure.Postgres;
using Moq;
using Testcontainers.PostgreSql;

namespace MoneyTracker.Queries.Tests.UserTests.Repository.Postgres;
public sealed class GetLastUserTokenForUserTest : IAsyncLifetime
{
    public readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
#if RUN_LOCAL
        .WithDockerEndpoint("tcp://localhost:2375")
#endif
        .WithImage("postgres:16")
        .WithCleanUp(true)
        .Build();

    public IUserDatabase _userRepository;
    public Mock<IDateTimeProvider> _mockDateTimeProvider;

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();

        _mockDateTimeProvider = new Mock<IDateTimeProvider>();
        var database = new PostgresDatabase(_postgres.GetConnectionString());

        Migration.CheckMigration(_postgres.GetConnectionString(), new MigrationOption(true));
        await database.UpdateTable("insert into user_id_to_token values (1, 'token 1', '2025-02-03 23:24:13.126961+00')");
        await database.UpdateTable("insert into user_id_to_token values (1, 'token 2', '2025-02-04 23:24:13.126961+00')");
        await database.UpdateTable("insert into user_id_to_token values (1, 'token 3', '2025-02-02 23:24:13.126961+00')");

        _userRepository = new UserDatabase(database, _mockDateTimeProvider.Object);
    }

    public Task DisposeAsync()
    {
        return _postgres.DisposeAsync().AsTask();
    }

    [Fact]
    public async Task SuccessfullyGetLastToken()
    {
        Assert.Equal("token 2", await _userRepository.GetLastUserTokenForUser(new UserEntity(1, "", "")));
    }

    [Fact]
    public async Task FailToGetTokenForUsersNotInDb()
    {
        Assert.Null(await _userRepository.GetLastUserTokenForUser(new UserEntity(2, "", "")));
    }
}
