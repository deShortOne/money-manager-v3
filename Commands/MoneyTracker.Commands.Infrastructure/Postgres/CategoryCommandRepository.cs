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

    public async Task<bool> DoesCategoryExist(int categoryId)
    {
        var query = """
            SELECT 1
            FROM category
            WHERE id = @id;
            """;
        var queryParams = new List<DbParameter>()
        {
            new NpgsqlParameter("id", categoryId),
        };

        var reader = await _database.GetTable(query, queryParams);
        await reader.ReadAsync();

        return reader.HasRows;
    }
}
