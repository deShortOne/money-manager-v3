
using System.Data;
using System.Data.Common;
using MoneyTracker.Data.Global;
using MoneyTracker.Shared.Auth;
using MoneyTracker.Shared.Data;
using Npgsql;

namespace MoneyTracker.Data.Postgres;

public class UserAuthDatabase : IUserAuthDatabase
{
    private readonly IDatabase _database;
    public UserAuthDatabase(IDatabase db)
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

    public async Task<Guid> GenerateTempGuidForUser(AuthenticatedUser user, DateTime expiration)
    {
        var query = """
            INSERT INTO user_id_to_token VALUES
            (@userId, (SELECT gen_random_uuid()), @expire)
            RETURNING (token);
         """;
        var queryParams = new List<DbParameter>()
        {
            new NpgsqlParameter("userId", user.UserId),
            new NpgsqlParameter("expire", expiration),
        };

        using var reader = await _database.GetTable(query, queryParams);

        if (await reader.ReadAsync())
        {
            return reader.GetGuid("token");
        }
        throw new InvalidDataException("User does not exist!");
    }

    public async Task<TokenMapToUserDTO> GetUserFromGuid(Guid userGuid)
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
            return new TokenMapToUserDTO(reader.GetInt32("user_id"), reader.GetDateTime("expires"));
        }

        throw new InvalidDataException("Guid does not map to a user!");
    }
}
