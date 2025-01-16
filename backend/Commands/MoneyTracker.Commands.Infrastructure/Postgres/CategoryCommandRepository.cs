using System.Data;
using System.Data.Common;
using MoneyTracker.Commands.Domain.Entities.Category;
using MoneyTracker.Commands.Domain.Repositories;
using MoneyTracker.Common.Interfaces;
using Npgsql;

namespace MoneyTracker.Commands.Infrastructure.Postgres;
public class CategoryCommandRepository : ICategoryCommandRepository
{
    private readonly IDatabase _database;

    public CategoryCommandRepository(IDatabase db)
    {
        _database = db;
    }

    public async Task AddCategory(CategoryEntity category)
    {
        var queryGetIdOfCategoryName = """
            INSERT INTO category (id, name) VALUES
                (@categoryId, @categoryName);
            """;
        var queryGetIdOfCategoryNameParams = new List<DbParameter>()
        {
            new NpgsqlParameter("categoryId", category.Id),
            new NpgsqlParameter("categoryName", category.Name),
        };

        using var reader = await _database.GetTable(queryGetIdOfCategoryName, queryGetIdOfCategoryNameParams);
    }

    public async Task EditCategory(EditCategoryEntity editCategoryDTO)
    {
        var queryGetIdOfCategoryName = """
            UPDATE category
                SET name = @categoryName
            WHERE id = @id;
            """;
        var queryGetIdOfCategoryNameParams = new List<DbParameter>()
        {
            new NpgsqlParameter("id", editCategoryDTO.Id),
            new NpgsqlParameter("categoryName", editCategoryDTO.Name),
        };

        await _database.GetTable(queryGetIdOfCategoryName, queryGetIdOfCategoryNameParams);
    }

    public async Task DeleteCategory(int categoryId)
    {
        var query = """
            DELETE FROM category
            WHERE id = @id;
            """;
        var queryParams = new List<DbParameter>()
        {
            new NpgsqlParameter("id", categoryId),
        };

        // get category id
        await _database.UpdateTable(query, queryParams);
    }

    public async Task<CategoryEntity?> GetCategory(int categoryId)
    {
        var query = """
            SELECT id,
                name
            FROM category
            WHERE id = @category_id;
         """;
        var queryParams = new List<DbParameter>()
        {
            new NpgsqlParameter("category_id", categoryId),
        };

        using var reader = await _database.GetTable(query, queryParams);

        if (reader.Rows.Count != 0)
            return new CategoryEntity(
                reader.Rows[0].Field<int>("id"),
                reader.Rows[0].Field<string>("name")!);
        return null;

    }

    public async Task<int> GetLastCategoryId()
    {
        var query = """
            SELECT MAX(id) as last_id
            FROM category;
            """;

        var reader = await _database.GetTable(query);

        if (reader.Rows.Count != 0)
        {
            return reader.Rows[0].Field<int>("last_id");
        }
        return 0;
    }
}
