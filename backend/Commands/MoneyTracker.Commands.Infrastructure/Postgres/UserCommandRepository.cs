
using System.Data;
using System.Data.Common;
using MoneyTracker.Authentication.Entities;
using MoneyTracker.Commands.Domain.Repositories;
using MoneyTracker.Common.Interfaces;
using MoneyTracker.Common.Utilities.DateTimeUtil;
using Npgsql;

public class UserCommandRepository : IUserCommandRepository
{
    private readonly IDatabase _database;
    private readonly IDateTimeProvider _dateTimeProvider;

    public UserCommandRepository(IDatabase db, IDateTimeProvider dateTimeProvider)
    {
        _database = db;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task AddUser(UserEntity userLogin)
    {
        var query = """
            INSERT INTO users (id, name, password) VALUES
                (@id, @username, @password);
            """;
        var queryParams = new List<DbParameter>()
        {
            new NpgsqlParameter("id", userLogin.Id),
            new NpgsqlParameter("username", userLogin.UserName),
            new NpgsqlParameter("password", userLogin.Password),
        };

        await _database.GetTable(query, queryParams);
    }

    public async Task<int> GetLastUserId()
    {
        var query = """
            SELECT MAX(id) as last_id
            from users;
         """;
         using var reader = await _database.GetTable(query);

        return await reader.ReadAsync() ? reader.GetInt32("last_id") : 0;
    }

    public async Task<UserAuthentication?> GetUserAuthFromToken(string token)
    {
        var query = """
            SELECT user_id, expires, name, password
            FROM user_id_to_token
            INNER JOIN users
            ON id = user_id
            WHERE token = @token
         """;
        var queryParams = new List<DbParameter>()
        {
            new NpgsqlParameter("token", token),
        };
        using var reader = await _database.GetTable(query, queryParams);

        if (await reader.ReadAsync())
        {
            var user = new UserEntity(reader.GetInt32("user_id"), reader.GetString("name"), reader.GetString("password"));
            return new UserAuthentication(user, token, reader.GetDateTime("expires"), _dateTimeProvider);
        }

        return null;
    }

    public async Task<UserEntity?> GetUserByUsername(string username)
    {
        var query = """
            SELECT id, name, password
            FROM users
            WHERE name = @username;
            """;
        var queryParams = new List<DbParameter>()
        {
            new NpgsqlParameter("username", username),
        };

        using var reader = await _database.GetTable(query, queryParams);

        if (await reader.ReadAsync())
        {
            return new UserEntity(reader.GetInt32("id"), reader.GetString("name"), reader.GetString("password"));
        }

        return null;
    }

    public async Task StoreTemporaryTokenToUser(UserAuthentication userAuthentication)
    {
        var query = """
            INSERT INTO user_id_to_token (user_id, token, expires) VALUES
                (@user_id, @token, @expires);
            """;
        var queryParams = new List<DbParameter>()
        {
            new NpgsqlParameter("user_id", userAuthentication.User.Id),
            new NpgsqlParameter("token", userAuthentication.Token),
            new NpgsqlParameter("expires", userAuthentication.Expiration),
        };

        await _database.GetTable(query, queryParams);
    }
}
