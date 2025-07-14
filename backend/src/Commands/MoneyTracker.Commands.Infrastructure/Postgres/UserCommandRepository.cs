using System.Data;
using System.Data.Common;
using MoneyTracker.Authentication.Entities;
using MoneyTracker.Commands.Domain.Repositories;
using MoneyTracker.Common.Interfaces;
using MoneyTracker.Common.Utilities.DateTimeUtil;
using Npgsql;

namespace MoneyTracker.Commands.Infrastructure.Postgres;
public class UserCommandRepository : IUserCommandRepository
{
    private readonly IDatabase _database;
    private readonly IDateTimeProvider _dateTimeProvider;

    public UserCommandRepository(IDatabase db, IDateTimeProvider dateTimeProvider)
    {
        _database = db;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task AddUser(UserEntity userLogin, CancellationToken cancellationToken)
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

        await _database.GetTable(query, cancellationToken, queryParams);
    }

    public async Task<int> GetLastUserId(CancellationToken cancellationToken)
    {
        var query = """
            SELECT COALESCE(MAX(id), 0) as last_id
            from users;
         """;
        using var reader = await _database.GetTable(query, cancellationToken);

        return reader.Rows[0].Field<int>("last_id");
    }

    public async Task<UserAuthentication?> GetUserAuthFromToken(string token, CancellationToken cancellationToken)
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
        using var reader = await _database.GetTable(query, cancellationToken, queryParams);

        if (reader.Rows.Count != 0)
        {
            var currRow = reader.Rows[0];
            var user = new UserEntity(currRow.Field<int>("user_id"), currRow.Field<string>("name")!, currRow.Field<string>("password")!);
            return new UserAuthentication(user, token, currRow.Field<DateTime>("expires"), _dateTimeProvider);
        }

        return null;
    }

    public async Task<UserEntity?> GetUserByUsername(string username, CancellationToken cancellationToken)
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

        using var reader = await _database.GetTable(query, cancellationToken, queryParams);

        if (reader.Rows.Count != 0)
        {
            var currRow = reader.Rows[0];
            return new UserEntity(currRow.Field<int>("id"), currRow.Field<string>("name")!, currRow.Field<string>("password")!);
        }

        return null;
    }

    public async Task StoreTemporaryTokenToUser(UserAuthentication userAuthentication,
        CancellationToken cancellationToken)
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

        await _database.GetTable(query, cancellationToken, queryParams);
    }
}
