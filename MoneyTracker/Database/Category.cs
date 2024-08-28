using MoneyTracker.API.Models.Category;
using Npgsql;
using System.Data;

namespace MoneyTracker.API.Database
{
    public class Category
    {
        public async Task<List<CategoryDTO>> GetAllCategories()
        {
            // UPSERTS!! and gets id
            var queryGetAllCategories = """
                SELECT id, name
                FROM category;
                """;

            // get category id
            using var reader = await Helper.GetTable(queryGetAllCategories);

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
            var queryGetIdOfCategoryNameParams = new List<NpgsqlParameter>()
            {
                new NpgsqlParameter("categoryName", categoryName.Name),
            };

            // get category id
            using var reader = await Helper.GetTable(queryGetIdOfCategoryName, queryGetIdOfCategoryNameParams);
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
            var queryGetIdOfCategoryNameParams = new List<NpgsqlParameter>()
            {
                new NpgsqlParameter("id", editCategoryDTO.Id),
                new NpgsqlParameter("categoryName", editCategoryDTO.Name),
            };

            // get category id
            using var reader = await Helper.GetTable(queryGetIdOfCategoryName, queryGetIdOfCategoryNameParams);
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
    }
}
