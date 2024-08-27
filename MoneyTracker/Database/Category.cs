using MoneyTracker.API.Models;
using Npgsql;
using System.Collections;
using System.Data;

namespace MoneyTracker.API.Database
{
    public class Category
    {
        public async Task<int> AddCategory(string categoryName)
        {
            // UPSERTS!! and gets id
            var queryGetIdOfCategoryName = """
                INSERT INTO category (name) VALUES
                    (@categoryName)
                ON CONFLICT (name) DO NOTHING
                RETURNING id;
                """;
            var queryGetIdOfCategoryNameParams = new List<NpgsqlParameter>()
            {
                new NpgsqlParameter("categoryName", categoryName),
            };

            // get category id
            using var readerId = await Helper.GetTable(queryGetIdOfCategoryName, queryGetIdOfCategoryNameParams);
            await readerId.ReadAsync();
            if (readerId == null || !readerId.HasRows)
            {
                throw new InvalidOperationException("Category name already exists");
            }
            return readerId.GetInt32(0);
        }

        public async Task<List<MoneyTracker.API.Models.Category>> GetAllCategories()
        {
            // UPSERTS!! and gets id
            var queryGetAllCategories = """
                SELECT id, name
                FROM category;
                """;

            // get category id
            using var reader = await Helper.GetTable(queryGetAllCategories);

            var res = new List<MoneyTracker.API.Models.Category>();
            while (await reader.ReadAsync())
            {
                res.Add(new MoneyTracker.API.Models.Category()
                {
                    Id = reader.GetInt32("id"),
                    Name = reader.GetString("name"),
                });
            }
            return res;
        }
    }
}
