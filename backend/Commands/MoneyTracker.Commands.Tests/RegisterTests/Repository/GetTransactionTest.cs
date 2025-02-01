
using MoneyTracker.Commands.DatabaseMigration;
using MoneyTracker.Commands.DatabaseMigration.Models;
using MoneyTracker.Commands.Domain.Entities.Transaction;

namespace MoneyTracker.Commands.Tests.RegisterTests.Repository;
public sealed class GetTransactionTest : RegisterRespositoryTestHelper
{
    [Fact]
    public async Task SuccessfullyGetTransaction()
    {
        Migration.CheckMigration(_postgres.GetConnectionString(), new MigrationOption(true));

        Assert.Equal(new TransactionEntity(1, 4, 1800, new DateOnly(2024, 08, 28), 1, 1), await _registerRepo.GetTransaction(1));
        Assert.Equal(new TransactionEntity(2, 5, 10, new DateOnly(2024, 08, 01), 2, 1), await _registerRepo.GetTransaction(2));
        Assert.Equal(new TransactionEntity(3, 6, 500, new DateOnly(2024, 08, 01), 3, 1), await _registerRepo.GetTransaction(3));
        Assert.Equal(new TransactionEntity(4, 7, 25, new DateOnly(2024, 08, 01), 4, 2), await _registerRepo.GetTransaction(4));
        Assert.Equal(new TransactionEntity(5, 7, 23, new DateOnly(2024, 08, 08), 4, 2), await _registerRepo.GetTransaction(5));
        Assert.Equal(new TransactionEntity(6, 7, 27, new DateOnly(2024, 08, 15), 4, 2), await _registerRepo.GetTransaction(6));
        Assert.Equal(new TransactionEntity(7, 8, 150, new DateOnly(2024, 08, 09), 5, 1), await _registerRepo.GetTransaction(7));
        Assert.Equal(new TransactionEntity(8, 4, 1500, new DateOnly(2024, 08, 28), 1, 3), await _registerRepo.GetTransaction(8));
        Assert.Equal(new TransactionEntity(9, 9, 75, new DateOnly(2024, 08, 29), 6, 3), await _registerRepo.GetTransaction(9));
        Assert.Equal(new TransactionEntity(10, 10, 100, new DateOnly(2024, 08, 30), 5, 3), await _registerRepo.GetTransaction(10));
    }

    [Fact]
    public async Task SuccessfullyGetNoTransaction()
    {
        Migration.CheckMigration(_postgres.GetConnectionString(), new MigrationOption(dropAllTables: true));

        Assert.Null(await _registerRepo.GetTransaction(1));
        Assert.Null(await _registerRepo.GetTransaction(2));
    }
}
