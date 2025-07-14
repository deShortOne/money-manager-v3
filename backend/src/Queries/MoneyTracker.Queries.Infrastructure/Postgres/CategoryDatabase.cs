using System.Data;
using MoneyTracker.Common.Interfaces;
using MoneyTracker.Common.Result;
using MoneyTracker.Queries.Domain.Entities.Category;
using MoneyTracker.Queries.Domain.Repositories.Database;

namespace MoneyTracker.Queries.Infrastructure.Postgres;
public class CategoryDatabase : ICategoryDatabase
{
    private readonly IDatabase _database;

    public CategoryDatabase(IDatabase db)
    {
        _database = db;
    }

    public async Task<ResultT<List<CategoryEntity>>> GetAllCategories(CancellationToken cancellationToken)
    {
        var queryGetAllCategories = """
            SELECT id, name
            FROM category
            ORDER BY name;
         """;

        using var reader = await _database.GetTable(queryGetAllCategories, cancellationToken);

        var res = new List<CategoryEntity>();
        foreach (DataRow row in reader.Rows)
        {
            res.Add(new CategoryEntity(row.Field<int>("id"), row.Field<string>("name")!));
        }
        return res;
    }
}
