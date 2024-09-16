
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

    public async Task<AuthenticatedUser> AuthenticateUser(UnauthenticatedUser userToLogIn)
    {
        var query = """
            SELECT id
            FROM users
            WHERE name = @username
         """;
        var queryParams = new List<DbParameter>()
        {
            new NpgsqlParameter("username", userToLogIn.Username),
        };

        using var reader = await _database.GetTable(query, queryParams);

        if (await reader.ReadAsync())
        {
            return new AuthenticatedUser(reader.GetInt32("id"));
        }

        throw new InvalidDataException("User does not exist!");
    }
}
