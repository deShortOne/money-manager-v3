
using System.Data;
using System.Data.Common;
using MoneyTracker.Authentication.Entities;
using MoneyTracker.Common.Interfaces;
using MoneyTracker.Common.Utilities.DateTimeUtil;
using Npgsql;

public class UserRepository : IUserRepository
{
    private readonly IDatabase _database;
    private readonly IDateTimeProvider _dateTimeProvider;

    public UserRepository(IDatabase db, IDateTimeProvider dateTimeProvider)
    {
        _database = db;
        _dateTimeProvider = dateTimeProvider;
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
    public async Task<string?> GetUserToken(UserEntity user)
    {
        var query = """
            SELECT token
            FROM user_id_to_token
            WHERE user_id = @userId;
            """;
        var queryParams = new List<DbParameter>()
        {
            new NpgsqlParameter("userId", user.Id),
        };

        using var reader = await _database.GetTable(query, queryParams);

        if (await reader.ReadAsync())
        {
            return reader.GetString("token");
        }

        return null;
    }

}