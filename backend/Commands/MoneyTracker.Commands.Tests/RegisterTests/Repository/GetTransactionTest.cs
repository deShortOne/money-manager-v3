using MoneyTracker.Commands.Domain.Entities.Transaction;
using MoneyTracker.Commands.Infrastructure.Postgres;
using MoneyTracker.Commands.Tests.Fixture;

namespace MoneyTracker.Commands.Tests.RegisterTests.Repository;
public sealed class GetTransactionTest : IClassFixture<PostgresDbFixture>
{
    private RegisterCommandRepository _registerRepo;

    public GetTransactionTest(PostgresDbFixture postgresFixture)
    {
        var _database = new PostgresDatabase(postgresFixture.ConnectionString);
        _registerRepo = new RegisterCommandRepository(_database);
    }

    [Fact]
    public async Task SuccessfullyGetTransaction()
    {
        Assert.Equal(new TransactionEntity(1, 9, 1800, new DateOnly(2024, 08, 28), 1, 1), await _registerRepo.GetTransaction(1));
        Assert.Equal(new TransactionEntity(2, 8, 10, new DateOnly(2024, 08, 01), 2, 1), await _registerRepo.GetTransaction(2));
        Assert.Equal(new TransactionEntity(3, 6, 500, new DateOnly(2024, 08, 01), 3, 1), await _registerRepo.GetTransaction(3));
        Assert.Equal(new TransactionEntity(4, 4, 25, new DateOnly(2024, 08, 01), 4, 3), await _registerRepo.GetTransaction(4));
        Assert.Equal(new TransactionEntity(5, 4, 23, new DateOnly(2024, 08, 08), 4, 3), await _registerRepo.GetTransaction(5));
        Assert.Equal(new TransactionEntity(6, 4, 27, new DateOnly(2024, 08, 15), 4, 3), await _registerRepo.GetTransaction(6));
        Assert.Equal(new TransactionEntity(7, 5, 150, new DateOnly(2024, 08, 09), 5, 1), await _registerRepo.GetTransaction(7));
        Assert.Equal(new TransactionEntity(8, 17, 1500, new DateOnly(2024, 08, 28), 1, 2), await _registerRepo.GetTransaction(8));
        Assert.Equal(new TransactionEntity(9, 18, 75, new DateOnly(2024, 08, 29), 6, 2), await _registerRepo.GetTransaction(9));
        Assert.Equal(new TransactionEntity(10, 19, 100, new DateOnly(2024, 08, 30), 5, 2), await _registerRepo.GetTransaction(10));
    }

    [Fact]
    public async Task SuccessfullyGetNoTransaction()
    {
        Assert.Null(await _registerRepo.GetTransaction(-1));
        Assert.Null(await _registerRepo.GetTransaction(-2));
    }
}
