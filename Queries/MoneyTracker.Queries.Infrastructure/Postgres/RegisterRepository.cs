using System.Data;
using System.Data.Common;
using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Common.Interfaces;
using MoneyTracker.Queries.Domain.Entities.Transaction;
using MoneyTracker.Queries.Domain.Repositories;
using Npgsql;

namespace MoneyTracker.Queries.Infrastructure.Postgres;
public class RegisterRepository : IRegisterRepository
{
    private readonly IDatabase _database;

    public RegisterRepository(IDatabase db)
    {
        _database = db;
    }

    public async Task<List<TransactionEntity>> GetAllTransactions(AuthenticatedUser user)
    {
        var query = """
            SELECT register.id,
                   payee,
                   amount,
                   datePaid,
                   c.name category_name,
                   account.name account_name
            FROM register
            INNER JOIN category c
                ON register.category_id = c.id
            INNER JOIN account
            	ON account.id = register.account_id 
            WHERE account.users_id = @user_id
            ORDER BY datePaid DESC,
               	c.id ASC;
            """;
        var queryParams = new List<DbParameter>()
        {
            new NpgsqlParameter("user_id", user.Id),
        };

        using var reader = await _database.GetTable(query, queryParams);

        var res = new List<TransactionEntity>();
        while (await reader.ReadAsync())
        {
            res.Add(new TransactionEntity(
                reader.GetInt32("id"),
                reader.GetString("payee"),
                reader.GetDecimal("amount"),
                DateOnly.FromDateTime(reader.GetDateTime("datePaid")),
                reader.GetString("category_name"),
                reader.GetString("account_name")
            ));
        }
        return res;
    }
}
