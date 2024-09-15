
using MoneyTracker.Core;
using MoneyTracker.Data.Postgres;
using MoneyTracker.DatabaseMigration;
using MoneyTracker.DatabaseMigration.Models;
using MoneyTracker.Shared.Data;
using MoneyTracker.Shared.Models.RepositoryToService.Bill;
using MoneyTracker.Shared.User;
using Testcontainers.PostgreSql;

namespace MoneyTracker.Tests.Authentication;
public sealed class UserAuthentication : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
#if RUN_LOCAL
        .WithDockerEndpoint("tcp://localhost:2375")
#endif
        .WithImage("postgres:16")
        .WithCleanUp(true)
        .Build();

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();

        Migration.CheckMigration(_postgres.GetConnectionString(), new MigrationOption(true));

        return;
    }

    public Task DisposeAsync()
    {
        return _postgres.DisposeAsync().AsTask();
    }

    [Fact]
    public async void SuccessfullyLogInUser()
    {
        var userToAuthenticate = new UnauthenticatedUser("root");
        var expected = new AuthenticatedUser(1);

        var db = new PostgresDatabase(_postgres.GetConnectionString());
        var userDb = new UserAuthDatabase(db);
        var userAuthService = new UserAuthenticationService(userDb);

        Assert.Equal(expected, await userAuthService.AuthenticateUser(userToAuthenticate));
    }

    [Fact]
    public async void FailToLogInUserThatDoesntExist()
    {
        var userToAuthenticate = new UnauthenticatedUser("broken root");

        var db = new PostgresDatabase(_postgres.GetConnectionString());
        var userDb = new UserAuthDatabase(db);
        var userAuthService = new UserAuthenticationService(userDb);

        await Assert.ThrowsAsync<InvalidDataException>(async () =>
        {
            await userAuthService.AuthenticateUser(userToAuthenticate);
        });
    }
}
