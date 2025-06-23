using MoneyTracker.Commands.Domain.Entities.Account;
using MoneyTracker.Commands.Domain.Repositories;
using MoneyTracker.Commands.Infrastructure.Postgres;
using MoneyTracker.Commands.Tests.Fixture;

namespace MoneyTracker.Commands.Tests.AccountTests.Repository;
public class GetAccountByIdTest : IClassFixture<PostgresDbFixture>
{
    public IAccountCommandRepository _accountRepo;

    public GetAccountByIdTest(PostgresDbFixture postgresDbFixture)
    {
        var _database = new PostgresDatabase(postgresDbFixture.ConnectionString);
        _accountRepo = new AccountCommandRepository(_database);
    }

    [Fact]
    public async Task GetAccountId1()
    {
        Assert.Equal(new AccountEntity(1, "Bank A"), await _accountRepo.GetAccountById(1));
    }

    [Fact]
    public async Task GetAccountId2()
    {
        Assert.Equal(new AccountEntity(2, "Bank B"), await _accountRepo.GetAccountById(2));
    }

    [Fact]
    public async Task GetAccountId3()
    {
        Assert.Equal(new AccountEntity(3, "Supermarket"), await _accountRepo.GetAccountById(3));
    }

    [Fact]
    public async Task FailToGetInvalidAccount()
    {
        Assert.Null(await _accountRepo.GetAccountById(-1));
    }
}
