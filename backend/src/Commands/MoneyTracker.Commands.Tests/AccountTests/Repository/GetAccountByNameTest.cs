using MoneyTracker.Commands.Domain.Entities.Account;
using MoneyTracker.Commands.Domain.Repositories;
using MoneyTracker.Commands.Infrastructure.Postgres;
using MoneyTracker.Commands.Tests.Fixture;

namespace MoneyTracker.Commands.Tests.AccountTests.Repository;
public class GetAccountByNameTest : IClassFixture<PostgresDbFixture>
{
    public IAccountCommandRepository _accountRepo;

    public GetAccountByNameTest(PostgresDbFixture postgresDbFixture)
    {
        var _database = new PostgresDatabase(postgresDbFixture.ConnectionString);
        _accountRepo = new AccountCommandRepository(_database);
    }

    [Fact]
    public async Task GetBankA()
    {
        Assert.Equal(new AccountEntity(1, "Bank A"), await _accountRepo.GetAccountByName("Bank A"));
    }

    [Fact]
    public async Task GetHobbyItem()
    {
        Assert.Equal(new AccountEntity(4, "Hobby Item"), await _accountRepo.GetAccountByName("Hobby Item"));
    }

    [Fact]
    public async Task GetFootballKit()
    {
        Assert.Equal(new AccountEntity(10, "Football Kit"), await _accountRepo.GetAccountByName("Football Kit"));
    }

    [Fact]
    public async Task FailToGetAnAccountThatDoesntExistAHHH()
    {
        Assert.Null(await _accountRepo.GetAccountByName("An account that doesn't exist AHHH"));
    }
}
