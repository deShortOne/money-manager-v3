using Npgsql;

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
    }
}
