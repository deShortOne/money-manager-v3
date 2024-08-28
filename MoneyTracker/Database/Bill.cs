using MoneyTracker.API.Models;
using System.Data;

namespace MoneyTracker.API.Database
{
    public class Bill
    {
        public async Task<List<BillDTO>> GetAllBills()
        {
            var query = """
                SELECT bill.id,
                	payee,
                	amount,
                	datePaid,
                	c.name
                FROM bill
                INNER JOIN category c
                	ON bill.category_id = c.id
                ORDER BY datePaid DESC;
                
                """;

            // get category id
            using var reader = await Helper.GetTable(query);

            var res = new List<BillDTO>();
            while (await reader.ReadAsync())
            {
                res.Add(new BillDTO()
                {
                    Id = reader.GetInt32("id"),
                    Payee = reader.GetString("payee"),
                    Amount = reader.GetDecimal("amount"),
                    DatePaid = reader.GetDateTime("datePaid"),
                    Category = reader.GetString("name"),
                });
            }
            return res;
        }
    }
}
