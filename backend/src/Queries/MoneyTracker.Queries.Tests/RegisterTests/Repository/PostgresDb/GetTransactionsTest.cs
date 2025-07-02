using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Queries.Domain.Entities.Transaction;
using MoneyTracker.Queries.Infrastructure.Postgres;
using MoneyTracker.Queries.Tests.Fixture;

namespace MoneyTracker.Queries.Tests.RegisterTests.Repository.PostgresDb;
public sealed class GetTransactionsTest : IClassFixture<PostgresDbFixture>
{
    private readonly PostgresDbFixture _postgresFixture;

    public GetTransactionsTest(PostgresDbFixture postgresFixture)
    {
        _postgresFixture = postgresFixture;
    }

    [Fact]
    public async Task FirstLoadCheckTablesThatDataAreThereForUserId1()
    {
        var db = new PostgresDatabase(_postgresFixture.ConnectionString);
        var registerDb = new RegisterDatabase(db);

        var actual = await registerDb.GetAllTransactions(new AuthenticatedUser(1), CancellationToken.None);

        var expected = new List<TransactionEntity>()
        {
            new(1, 4, "Company A", 1800, new DateOnly(2024, 8, 28), 1, "Wages & Salary : Net Pay", 1, "bank a"),
            new(6, 7, "Supermarket", 27, new DateOnly(2024, 8, 15), 4, "Groceries", 2, "bank b"),
            new(7, 8, "Hobby item", 150, new DateOnly(2024, 8, 9), 5, "Hobby", 1, "bank a"),
            new(5, 7, "Supermarket", 23, new DateOnly(2024, 8, 8), 4, "Groceries", 2, "bank b"),
            new(2, 5, "Phone company", 10, new DateOnly(2024, 8, 1), 2, "Bills : Cell Phone", 1, "bank a"),
            new(3, 6, "Landlord A", 500, new DateOnly(2024, 8, 1), 3, "Bills : Rent", 1, "bank a"),
            new(4, 7, "Supermarket", 25, new DateOnly(2024, 8, 1), 4, "Groceries", 2, "bank b"),
        };

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task FirstLoadCheckTablesThatDataAreThereForUserId2()
    {
        var db = new PostgresDatabase(_postgresFixture.ConnectionString);
        var registerDb = new RegisterDatabase(db);

        var actual = await registerDb.GetAllTransactions(new AuthenticatedUser(2), CancellationToken.None);

        var expected = new List<TransactionEntity>()
        {
            new(10, 10, "Football kit", 100, new DateOnly(2024, 8, 30), 5, "Hobby", 3, "bank a"),
            new(9, 9, "Vet", 75, new DateOnly(2024, 8, 29), 6, "Pet Care", 3, "bank a"),
            new(8, 4, "Company A", 1500, new DateOnly(2024, 8, 28), 1, "Wages & Salary : Net Pay", 3, "bank a"),
        };

        Assert.Equal(expected, actual);
    }
}
