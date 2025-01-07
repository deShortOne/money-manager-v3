using System.Data;
using MoneyTracker.Common.Interfaces;
using MoneyTracker.Queries.Domain.Entities.Category;
using MoneyTracker.Queries.Domain.Repositories;
using Npgsql;

namespace MoneyTracker.Queries.Infrastructure.Postgres;
public class CategoryRepository : ICategoryRepository
{
    private readonly IDatabase _database;

    public CategoryRepository(IDatabase db)
    {
        _database = db;
    }

    public async Task<List<CategoryEntity>> GetAllCategories()
    {
        // UPSERTS!! and gets id
        var queryGetAllCategories = """
            SELECT id, name
            FROM category
            ORDER BY name;
         """;

        // get category id
        using var reader = await _database.GetTable(queryGetAllCategories);

        var res = new List<CategoryEntity>();
        foreach (DataRow row in reader.Rows)
        {
            res.Add(new CategoryEntity(row.Field<int>("id"), row.Field<string>("name")!));
        }
        return res;
    }
}
