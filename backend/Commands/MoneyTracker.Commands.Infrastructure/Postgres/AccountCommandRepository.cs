using System.Data;
using System.Data.Common;
using MoneyTracker.Commands.Domain.Entities.Account;
using MoneyTracker.Commands.Domain.Repositories;
using MoneyTracker.Common.Interfaces;
using Npgsql;

namespace MoneyTracker.Commands.Infrastructure.Postgres;
public class AccountCommandRepository : IAccountCommandRepository
{
    private readonly IDatabase _database;
    public AccountCommandRepository(IDatabase db)
    {
        _database = db;
    }

    public async Task AddAccount(AccountEntity newAccount)
    {
        string query = """
            INSERT INTO account (id, name)
            VALUES (@id, @name);
            """;
        var queryParams = new List<DbParameter>()
            {
                new NpgsqlParameter("id", newAccount.Id),
                new NpgsqlParameter("name", newAccount.Name),
            };

        await _database.UpdateTable(query, queryParams);
    }

    public async Task AddAccountToUser(AccountUserEntity newAccountUserEntity)
    {
        string query = """
            INSERT INTO account_user (id, account_id, users_id, user_owns_account)
            VALUES (@id, @account_id, @users_id, @user_owns_account);
            """;
        var queryParams = new List<DbParameter>()
        {
                new NpgsqlParameter("id", newAccountUserEntity.Id),
                new NpgsqlParameter("account_id", newAccountUserEntity.AccountId),
                new NpgsqlParameter("users_id", newAccountUserEntity.UserId),
                new NpgsqlParameter("user_owns_account", newAccountUserEntity.UserOwnsAccount),
            };

        await _database.UpdateTable(query, queryParams);
    }

    public async Task<AccountEntity?> GetAccountById(int accountId)
    {
        var query = """
            SELECT id,
                name
            FROM account
            WHERE id = @account_id;
         """;
        var queryParams = new List<DbParameter>()
        {
            new NpgsqlParameter("account_id", accountId),
        };

        using var reader = await _database.GetTable(query, queryParams);

        if (reader.Rows.Count != 0)
            return new AccountEntity(
                reader.Rows[0].Field<int>("id"),
                reader.Rows[0].Field<string>("name")!);
        return null;
    }

    public async Task<AccountEntity?> GetAccountByName(string accountName)
    {
        var query = """
            SELECT id,
                name
            FROM account
            WHERE name = @account_name;
         """;
        var queryParams = new List<DbParameter>()
        {
            new NpgsqlParameter("account_name", accountName),
        };

        using var reader = await _database.GetTable(query, queryParams);

        if (reader.Rows.Count != 0)
            return new AccountEntity(
                reader.Rows[0].Field<int>("id"),
                reader.Rows[0].Field<string>("name")!);
        return null;
    }

    public async Task<AccountUserEntity?> GetAccountUserEntity(int accountId, int userId)
    {
        var query = """
            SELECT id,
                users_id,
                account_id,
                user_owns_account
            FROM account_user
            WHERE users_id = @users_id
                AND account_id = @account_id;
         """;
        var queryParams = new List<DbParameter>()
        {
            new NpgsqlParameter("users_id", userId),
            new NpgsqlParameter("account_id", accountId),
        };

        using var reader = await _database.GetTable(query, queryParams);

        if (reader.Rows.Count != 0)
            return new AccountUserEntity(
                reader.Rows[0].Field<int>("id"),
                reader.Rows[0].Field<int>("account_id"),
                reader.Rows[0].Field<int>("users_id")!,
                reader.Rows[0].Field<bool>("user_owns_account"));
        return null;
    }

    public Task<int> GetLastAccountId() => throw new NotImplementedException();
    public Task<int> GetLastAccountUserId() => throw new NotImplementedException();
}
