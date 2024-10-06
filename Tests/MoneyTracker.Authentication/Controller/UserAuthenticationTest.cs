
//using Microsoft.AspNetCore.Http;
//using MoneyTracker.API.Controllers;
//using MoneyTracker.Core;
//using MoneyTracker.Data.Postgres;
//using MoneyTracker.DatabaseMigration;
//using MoneyTracker.DatabaseMigration.Models;
//using MoneyTracker.Shared.Auth;
//using MoneyTracker.Shared.DateManager;
//using Moq;
//using Npgsql;
//using Testcontainers.PostgreSql;

//namespace MoneyTracker.Tests.Authentication;

//public sealed class UserAuthenticationTest : IAsyncLifetime
//{
//    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
//#if RUN_LOCAL
//        .WithDockerEndpoint("tcp://localhost:2375")
//#endif
//        .WithImage("postgres:16")
//        .WithCleanUp(true)
//        .Build();

//    private UserAuthenticationController _userAuthController;

//    public async Task InitializeAsync()
//    {
//        await _postgres.StartAsync();
//        Migration.CheckMigration(_postgres.GetConnectionString(), new MigrationOption(true));

//        var db = new PostgresDatabase(_postgres.GetConnectionString());
//        var userDb = new UserAuthDatabase(db);
//        var jwtToken = new JwtConfig("", "", "", 0);
//        var userAuthService = new UserAuthenticationService(userDb, jwtToken, new DateTimeProvider(), new PasswordHasher());
//        var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
//        var httpContext = new DefaultHttpContext();
//        httpContext.Request.Headers.Authorization = "Bearer test-token-fds1200";
//        mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(httpContext);
//        _userAuthController = new UserAuthenticationController(null, userAuthService, mockHttpContextAccessor.Object);

//        return;
//    }

//    public Task DisposeAsync()
//    {
//        return _postgres.DisposeAsync().AsTask();
//    }

//    [Fact]
//    public async void SuccessfullyLogInUser()
//    {
//        var userToAuthenticate = new LoginWithUsernameAndPassword("root", "root-pass");
//        var expected = new AuthenticatedUser(1);

//        Assert.Equal(expected, await _userAuthController.AuthenticateUser(userToAuthenticate));
//    }

//    [Fact]
//    public async void FailToLogInUserThatDoesntExist()
//    {
//        var userToAuthenticate = new LoginWithUsernameAndPassword("broken root");

//        var db = new PostgresDatabase(_postgres.GetConnectionString());
//        var userDb = new UserAuthDatabase(db);
//        var jwtToken = new JwtConfig("", "", "", 0);
//        var userAuthService = new UserAuthenticationService(userDb, jwtToken, new DateTimeProvider());

//        await Assert.ThrowsAsync<InvalidDataException>(async () =>
//        {
//            await userAuthService.AuthenticateUser(userToAuthenticate);
//        });
//    }

//    [Fact]
//    public async void GeneratesABearerTokenAndEnsureIsValid()
//    {
//        var db = new PostgresDatabase(_postgres.GetConnectionString());
//        var userDb = new UserAuthDatabase(db);
//        var jwtToken = new JwtConfig("iss_company a",
//            "aud_company b",
//            "TOPSECRETTOPSECRETTOPSECRETTOPSE",
//            5
//        );
//        var userAuthService = new UserAuthenticationService(userDb, jwtToken, new DateTimeProvider());

//        var userToAuth = new LoginWithUsernameAndPassword("root");
//        var token = await userAuthService.GenerateToken(userToAuth);
//        Assert.NotNull(token);

//        var expectedAuthedUser = new AuthenticatedUser(1);

//        var actualAuthedUser = await userAuthService.DecodeToken(token);
//        Assert.Equal(expectedAuthedUser, actualAuthedUser);

//        var dataTable = await db.GetTable("SELECT 1 FROM users WHERE id = @id AND name = @name", [new NpgsqlParameter("id", 1), new NpgsqlParameter("name", "root")]);
//        Assert.True(await dataTable.ReadAsync());
//    }

//    [Fact]
//    public async void GeneratesABearerTokenAndEnsureIsValidAcrossServiceMadeTwice()
//    {
//        var db = new PostgresDatabase(_postgres.GetConnectionString());
//        var userDb = new UserAuthDatabase(db);
//        var jwtToken = new JwtConfig("iss_company a",
//            "aud_company b",
//            "TOPSECRETTOPSECRETTOPSECRETTOPSE",
//            5
//        );
//        var userAuthService = new UserAuthenticationService(userDb, jwtToken, new DateTimeProvider());

//        var userToAuth = new LoginWithUsernameAndPassword("root");
//        var token = await userAuthService.GenerateToken(userToAuth);
//        Assert.NotNull(token);

//        var expectedAuthedUser = new AuthenticatedUser(1);

//        var userAuthService2 = new UserAuthenticationService(userDb, jwtToken, new DateTimeProvider());
//        var actualAuthedUser = await userAuthService2.DecodeToken(token);
//        Assert.Equal(expectedAuthedUser, actualAuthedUser);

//        var dataTable = await db.GetTable("SELECT 1 FROM users WHERE id = @id AND name = @name", [new NpgsqlParameter("id", 1), new NpgsqlParameter("name", "root")]);
//        Assert.True(await dataTable.ReadAsync());
//    }


//    [Fact]
//    public async void AttemptToUseAnExpiredBearerToken()
//    {
//        var db = new PostgresDatabase(_postgres.GetConnectionString());
//        var userDb = new UserAuthDatabase(db);
//        var jwtToken = new JwtConfig("iss_company a",
//            "aud_company b",
//            "TOPSECRETTOPSECRETTOPSECRETTOPSE",
//            5
//        );
//        var mockDateTimeProvider = new Mock<IDateTimeProvider>();
//        mockDateTimeProvider.Setup(x => x.Now).Returns(new DateTimeProvider().Now.AddHours(-1));
//        var userAuthService = new UserAuthenticationService(userDb, jwtToken, mockDateTimeProvider.Object);

//        var userToAuth = new LoginWithUsernameAndPassword("root");
//        var token = await userAuthService.GenerateToken(userToAuth);
//        Assert.NotNull(token);

//        var expectedAuthedUser = new AuthenticatedUser(1);

//        var userAuthService2 = new UserAuthenticationService(userDb, jwtToken, new DateTimeProvider());
//        await Assert.ThrowsAsync<InvalidDataException>(async () =>
//        {
//            await userAuthService2.DecodeToken(token);
//        });
//    }
//}
