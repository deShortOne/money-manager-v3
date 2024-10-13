
//using System.IdentityModel.Tokens.Jwt;
//using Microsoft.AspNetCore.Http;
//using MoneyTracker.API.Controllers;
//using MoneyTracker.Core;
//using MoneyTracker.Data.Postgres;
//using MoneyTracker.DatabaseMigration;
//using MoneyTracker.DatabaseMigration.Models;
//using MoneyTracker.Shared.Auth;
//using MoneyTracker.Shared.DateManager;
//using MoneyTracker.Shared.Shared;
//using Moq;
//using Testcontainers.PostgreSql;

//namespace MoneyTracker.Tests.OldTestsToMoveOver.Controller;
//public sealed class UserAuthenticationTest : IAsyncLifetime
//{
//    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
//#if RUN_LOCAL
//        .WithDockerEndpoint("tcp://localhost:2375")
//#endif
//        .WithImage("postgres:16")
//        .WithCleanUp(true)
//        .Build();

//    public async Task InitializeAsync()
//    {
//        await _postgres.StartAsync();

//        Migration.CheckMigration(_postgres.GetConnectionString(), new MigrationOption(true));

//        return;
//    }

//    public Task DisposeAsync()
//    {
//        return _postgres.DisposeAsync().AsTask();
//    }

//    [Fact]
//    public void RepeatBackBearerToken()
//    {
//        var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
//        var httpContext = new DefaultHttpContext();
//        httpContext.Request.Headers.Authorization = "Bearer test-token-fds1200";
//        mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(httpContext);

//        var db = new PostgresDatabase(_postgres.GetConnectionString());
//        var userDb = new UserAuthRepository(db);
//        var jwtToken = new JwtConfig("", "", "", 0);
//        var userAuthService = new UserAuthenticationService(userDb,
//            jwtToken,
//            new DateTimeProvider(),
//            new PasswordHasher(),
//            new IdGenerator(),
//            new JwtSecurityTokenHandler());

//        var userAuthController = new UserAuthenticationController(null, userAuthService, mockHttpContextAccessor.Object);
//        var token = userAuthController.RepeatAuthTokenBack();

//        Assert.Equal("test-token-fds1200", token);
//    }
//}
