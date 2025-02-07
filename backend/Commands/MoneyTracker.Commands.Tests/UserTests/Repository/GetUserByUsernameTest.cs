using System.Data.Common;
using MoneyTracker.Authentication.Entities;
using MoneyTracker.Commands.DatabaseMigration;
using MoneyTracker.Commands.DatabaseMigration.Models;
using MoneyTracker.Commands.Infrastructure.Postgres;
using MoneyTracker.Commands.Tests.Fixture;
using MoneyTracker.Common.Interfaces;
using MoneyTracker.Common.Utilities.DateTimeUtil;
using Moq;
using Npgsql;
using Testcontainers.PostgreSql;

namespace MoneyTracker.Commands.Tests.UserTests.Repository;
public sealed class GetUserByUsernameTest : IClassFixture<PostgresDbFixture>
{

    private UserCommandRepository _userRepo;
    private IDatabase _database;

    public GetUserByUsernameTest(PostgresDbFixture postgresFixture)
    {
        _database = new PostgresDatabase(postgresFixture.ConnectionString);
        _userRepo = new UserCommandRepository(_database, Mock.Of<IDateTimeProvider>());
    }

    [Fact]
    public async Task GetUserRootByUsernameSuccessfully()
    {
        var expected = new UserEntity(1, "root", "IfC1pbsUdKwcX68HPvPybQ==.bfXuHix96vvlXfGqLpY+/kRgBnCbXCU/Kqu2uIY8M60=");

        var actual = await _userRepo.GetUserByUsername("root");
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task GetUserRoot2ByUsernameSuccessfully()
    {
        var expected = new UserEntity(2, "secondary root", "lH0GmZnlH6TAwD+2wQx1UA==.C4UPD8P66L/A4AKv77WTsN6CSl6Wobgyy0psL3OkO+s=");

        var actual = await _userRepo.GetUserByUsername("secondary root");
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task GetUserByUsernameFailDueToNameNotBeingInDatabase()
    {
        var actual = await _userRepo.GetUserByUsername("asd");
        Assert.Null(actual);
    }
}
