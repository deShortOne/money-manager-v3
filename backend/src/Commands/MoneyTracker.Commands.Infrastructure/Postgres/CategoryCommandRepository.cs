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

    public async Task AddCategory(CategoryEntity category, CancellationToken cancellationToken)
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

        using var reader = await _database.GetTable(queryGetIdOfCategoryName, cancellationToken, queryGetIdOfCategoryNameParams);
    }

    public async Task EditCategory(EditCategoryEntity editCategoryDTO, CancellationToken cancellationToken)
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

        await _database.GetTable(queryGetIdOfCategoryName, cancellationToken, queryGetIdOfCategoryNameParams);
    }

    public async Task DeleteCategory(int categoryId, CancellationToken cancellationToken)
    {
        var query = """
            DELETE FROM category
            WHERE id = @id;
            """;
        var queryParams = new List<DbParameter>()
        {
            new NpgsqlParameter("id", categoryId),
        };

        await _database.UpdateTable(query, cancellationToken, queryParams);
    }

    public async Task<CategoryEntity?> GetCategory(int categoryId, CancellationToken cancellationToken)
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

        using var reader = await _database.GetTable(query, cancellationToken, queryParams);

        if (reader.Rows.Count != 0)
            return new CategoryEntity(
                reader.Rows[0].Field<int>("id"),
                reader.Rows[0].Field<string>("name")!);
        return null;
    }

    public async Task<CategoryEntity?> GetCategory(string categoryName, CancellationToken cancellationToken)
    {
        var query = """
            SELECT id,
                name
            FROM category
            WHERE name = @category_name;
         """;
        var queryParams = new List<DbParameter>()
        {
            new NpgsqlParameter("category_name", categoryName),
        };

        using var reader = await _database.GetTable(query, cancellationToken, queryParams);

        if (reader.Rows.Count != 0)
            return new CategoryEntity(
                reader.Rows[0].Field<int>("id"),
                reader.Rows[0].Field<string>("name")!);
        return null;
    }


    public async Task<int> GetLastCategoryId(CancellationToken cancellationToken)
    {
        var query = """
            SELECT MAX(id) as last_id
            FROM category;
            """;

        var reader = await _database.GetTable(query, cancellationToken);

        if (reader.Rows.Count != 0)
        {
            return reader.Rows[0].Field<int>("last_id");
        }
        return 0;
    }
}
