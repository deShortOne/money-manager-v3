using MoneyTracker.API.Models;
using Npgsql;
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

        public async Task<BillDTO> AddNewBill(NewBillDTO bill)
        {
            var query = """
                INSERT INTO bill (payee, amount, datePaid, category_id) VALUES
                    (@payee, @amount, @datePaid, @category_id)
                RETURNING (id),
                    (payee), 
                    (amount), 
                    (datePaid), 
                    (SELECT name
                    FROM category
                    WHERE id = @category_id);
                """;
            var queryParams= new List<NpgsqlParameter>()
            {
                new NpgsqlParameter("payee", bill.Payee),
                new NpgsqlParameter("amount", bill.Amount),
                new NpgsqlParameter("datePaid", bill.DatePaid),
                new NpgsqlParameter("category_id", bill.Category),
            };
            using var reader = await Helper.GetTable(query, queryParams);
            if (await reader.ReadAsync())
            {
                return new BillDTO()
                {
                    Id = reader.GetInt32("id"),
                    Payee = reader.GetString("payee"),
                    Amount = reader.GetDecimal("amount"),
                    DatePaid = reader.GetDateTime("datePaid"),
                    Category = reader.GetString("name"),
                };
            }
            return null; //throw error
        }
    }
}
