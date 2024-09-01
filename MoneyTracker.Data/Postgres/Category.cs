using System.Data;
using System.Data.Common;
using MoneyTracker.Data.Global;
using MoneyTracker.Shared.Models.Category;
using Npgsql;

namespace MoneyTracker.Data.Postgres
{
    public class Category : ICategory
    {
        private readonly Helper _database;

        public Category(IHelper db)
        {
            _database = (Helper)db;
        }

        public async Task<List<CategoryDTO>> GetAllCategories()
        {
            // UPSERTS!! and gets id
            var queryGetAllCategories = """
                SELECT id, name
                FROM category;
                """;

            // get category id
            using var reader = await _database.GetTable(queryGetAllCategories);

            var res = new List<CategoryDTO>();
            while (await reader.ReadAsync())
            {
                res.Add(new CategoryDTO()
                {
                    Id = reader.GetInt32("id"),
                    Name = reader.GetString("name"),
                });
            }
            return res;
        }

        public async Task<CategoryDTO> AddCategory(NewCategoryDTO categoryName)
        {
            // UPSERTS!! and gets id
            var queryGetIdOfCategoryName = """
                INSERT INTO category (name) VALUES
                    (@categoryName)
                ON CONFLICT (name) DO NOTHING
                RETURNING (id), (name);
                """;
            var queryGetIdOfCategoryNameParams = new List<DbParameter>()
            {
                new NpgsqlParameter("categoryName", categoryName.Name),
            };

            // get category id
            using var reader = await _database.GetTable(queryGetIdOfCategoryName, queryGetIdOfCategoryNameParams);
            while (await reader.ReadAsync())
            {
                return new CategoryDTO()
                {
                    Id = reader.GetInt32("id"),
                    Name = reader.GetString("name"),
                };
            }
            return null; // throw error
        }

        public async Task<CategoryDTO> EditCategory(EditCategoryDTO editCategoryDTO)
        {
            // UPSERTS!! and gets id
            var queryGetIdOfCategoryName = """
                UPDATE category
                    SET name = @categoryName
                WHERE id = @id
                RETURNING (id), (name);
                """;
            var queryGetIdOfCategoryNameParams = new List<DbParameter>()
            {
                new NpgsqlParameter("id", editCategoryDTO.Id),
                new NpgsqlParameter("categoryName", editCategoryDTO.Name),
            };

            // get category id
            using var reader = await _database.GetTable(queryGetIdOfCategoryName, queryGetIdOfCategoryNameParams);
            if (await reader.ReadAsync())
            {
                return new CategoryDTO()
                {
                    Id = reader.GetInt32("id"),
                    Name = reader.GetString("name"),
                };
            }
            return null; // throw error
        }

        public async Task<bool> DeleteCategory(DeleteCategoryDTO deleteCategoryDTO)
        {
            // UPSERTS!! and gets id
            var query = """
                DELETE FROM category
                WHERE id = @id;
                """;
            var queryParams = new List<DbParameter>()
            {
                new NpgsqlParameter("id", deleteCategoryDTO.Id),
            };

            // get category id
            var reader = await _database.UpdateTable(query, queryParams);
            return reader == 1;
        }
    }
}
