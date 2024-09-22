
using MoneyTracker.Core;
using MoneyTracker.Data.Postgres;
using MoneyTracker.DatabaseMigration;
using MoneyTracker.DatabaseMigration.Models;
using MoneyTracker.Shared.Auth;
using Npgsql;
using Testcontainers.PostgreSql;

namespace MoneyTracker.Tests.Authentication;
public sealed class UserAuthenticationTest : IAsyncLifetime
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
        var userToAuthenticate = new LoginWithUsernameAndPassword("root");
        var expected = new AuthenticatedUser(1);

        var db = new PostgresDatabase(_postgres.GetConnectionString());
        var userDb = new UserAuthDatabase(db);
        var jwtToken = new JwtConfig("", "", "", 0);
        var userAuthService = new UserAuthenticationService(userDb, jwtToken);

        Assert.Equal(expected, await userAuthService.AuthenticateUser(userToAuthenticate));
    }

    [Fact]
    public async void FailToLogInUserThatDoesntExist()
    {
        var userToAuthenticate = new LoginWithUsernameAndPassword("broken root");

        var db = new PostgresDatabase(_postgres.GetConnectionString());
        var userDb = new UserAuthDatabase(db);
        var jwtToken = new JwtConfig("", "", "", 0);
        var userAuthService = new UserAuthenticationService(userDb, jwtToken);

        await Assert.ThrowsAsync<InvalidDataException>(async () =>
        {
            await userAuthService.AuthenticateUser(userToAuthenticate);
        });
    }


    [Fact]
    public async void GeneratesABearerToken()
    {
        var db = new PostgresDatabase(_postgres.GetConnectionString());
        var userDb = new UserAuthDatabase(db);
        var jwtToken = new JwtConfig("iss_company a",
            "aud_company b",
            "TOPSECRETTOPSECRETTOPSECRETTOPSE",
            0
        );
        var userAuthService = new UserAuthenticationService(userDb, jwtToken);

        var userToAuth = new LoginWithUsernameAndPassword("root");
        var token = await userAuthService.GenerateToken(userToAuth);
        Assert.NotNull(token);

        var expectedAuthedUser = new AuthenticatedUser(1);

        var actualAuthedUser = await userAuthService.DecodeToken(token);
        Assert.Equal(expectedAuthedUser, actualAuthedUser);

        var dataTable = await db.GetTable("SELECT 1 FROM users WHERE id = @id AND name = @name", [new NpgsqlParameter("id", 1), new NpgsqlParameter("name", "root")]);
        Assert.True(await dataTable.ReadAsync());
    }
}
