using System.Data;
using System.Data.Common;
using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Authentication.Entities;
using MoneyTracker.Authentication.Interfaces;
using MoneyTracker.Common.Interfaces;
using Npgsql;

namespace MoneyTracker.Commands.Infrastructure.Postgres;
public class UserAuthRepository : IUserAuthRepository
{
    private readonly IDatabase _database;
    public UserAuthRepository(IDatabase db)
    {
        _database = db;
    }

    public async Task<UserEntity?> GetUserByUsername(string username)
    {
        var query = """
            SELECT id, name, password
            FROM users
            WHERE name = @username
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

    public async Task StoreTemporaryTokenToUser(UserEntity user, Guid token, DateTime expiration)
    {
        var query = """
            INSERT INTO user_id_to_token VALUES
            (@userId, @token, @expire)
         """;
        var queryParams = new List<DbParameter>()
        {
            new NpgsqlParameter("userId", user.Id),
            new NpgsqlParameter("token", token),
            new NpgsqlParameter("expire", expiration),
        };

        using var reader = await _database.GetTable(query, queryParams);
    }

    public async Task<TokenMapToUserEntity?> GetUserFromToken(Guid userGuid)
    {
        var query = """
            SELECT user_id, expires
            FROM user_id_to_token
            WHERE token = @guid
         """;
        var queryParams = new List<DbParameter>()
        {
            new NpgsqlParameter("guid", userGuid),
        };
        using var reader = await _database.GetTable(query, queryParams);

        if (await reader.ReadAsync())
        {
            return new TokenMapToUserEntity(reader.GetInt32("user_id"), reader.GetDateTime("expires"));
        }

        return null;
    }

    public async Task StoreTemporaryTokenToUser(AuthenticatedUser user, Guid token, DateTime expiration)
    {
        var query = """
            INSERT INTO user_id_to_token VALUES
            (@userId, @token, @expire)
         """;
        var queryParams = new List<DbParameter>()
        {
            new NpgsqlParameter("userId", user.Id),
            new NpgsqlParameter("token", token),
            new NpgsqlParameter("expire", expiration),
        };

        using var reader = await _database.GetTable(query, queryParams);
    }
}
