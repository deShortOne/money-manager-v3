
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

        if (reader.Rows.Count != 0)
        {
            var currRow = reader.Rows[0];
            var user = new UserEntity(currRow.Field<int>("user_id"), currRow.Field<string>("name")!, currRow.Field<string>("password")!);
            return new UserAuthentication(user, token, currRow.Field<DateTime>("expires"), _dateTimeProvider);
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

        if (reader.Rows.Count != 0)
        {
            var currRow = reader.Rows[0];
            return new UserEntity(currRow.Field<int>("id"), currRow.Field<string>("name")!, currRow.Field<string>("password")!);
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

        if (reader.Rows.Count != 0)
        {
            return reader.Rows[0].Field<string>("token");
        }

        return null;
    }

}