using System.Data;
using System.Data.Common;
using MoneyTracker.Data.Global;
using MoneyTracker.Shared.Data;
using MoneyTracker.Shared.Models.RepositoryToService.Category;
using MoneyTracker.Shared.Models.ServiceToRepository.Category;
using Npgsql;

namespace MoneyTracker.Data.Postgres
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly IDatabase _database;

        public CategoryRepository(IDatabase db)
        {
            _database = db;
        }

        public async Task<List<CategoryEntityDTO>> GetAllCategories()
        {
            // UPSERTS!! and gets id
            var queryGetAllCategories = """
                SELECT id, name
                FROM category
                ORDER BY name;
             """;

            // get category id
            using var reader = await _database.GetTable(queryGetAllCategories);

            var res = new List<CategoryEntityDTO>();
            while (await reader.ReadAsync())
            {
                res.Add(new CategoryEntityDTO(reader.GetInt32("id"), reader.GetString("name")));
            }
            return res;
        }

        public async Task AddCategory(NewCategoryDTO categoryName)
        {
            var queryGetIdOfCategoryName = """
                INSERT INTO category (name) VALUES
                    (@categoryName);
                """;
            var queryGetIdOfCategoryNameParams = new List<DbParameter>()
            {
                new NpgsqlParameter("categoryName", categoryName.Name),
            };

            using var reader = await _database.GetTable(queryGetIdOfCategoryName, queryGetIdOfCategoryNameParams);
        }

        public async Task EditCategory(EditCategoryDTO editCategoryDTO)
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

        public async Task DeleteCategory(DeleteCategoryDTO deleteCategoryDTO)
        {
            var query = """
                DELETE FROM category
                WHERE id = @id;
                """;
            var queryParams = new List<DbParameter>()
            {
                new NpgsqlParameter("id", deleteCategoryDTO.Id),
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
}
