using MoneyTracker.API.Models.Bill;
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
            var queryParams = new List<NpgsqlParameter>()
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

        public async Task<BillDTO> EditBill(EditBillDTO bill)
        {
            var setParamsLis = new List<string>();
            var queryParams = new List<NpgsqlParameter>()
            {
                new NpgsqlParameter("id", bill.Id),
            };
            if (bill.Payee != null)
            {
                setParamsLis.Add("payee = @payee");
                queryParams.Add(new NpgsqlParameter("payee", bill.Payee));
            }
            if (bill.Amount != null)
            {
                setParamsLis.Add("amount = @amount");
                queryParams.Add(new NpgsqlParameter("amount", bill.Amount));
            }
            if (bill.DatePaid != null)
            {
                setParamsLis.Add("datePaid = @datePaid");
                queryParams.Add(new NpgsqlParameter("datePaid", bill.DatePaid));
            }
            if (bill.Category != null)
            {
                setParamsLis.Add("category_id = @category_id");
                queryParams.Add(new NpgsqlParameter("category_id", bill.Category));
            }

            if (setParamsLis.Count == 0)
            {
                throw new ArgumentException("Value to update must exist");
            }

            var query = $"""
                UPDATE bill 
                    SET {string.Join(",", setParamsLis)}
                WHERE id = @id
                RETURNING (id),
                    (payee), 
                    (amount), 
                    (datePaid), 
                    (SELECT name
                    FROM category
                    WHERE id = @category_id);
                """;

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

        public async Task<bool> DeleteBill(DeleteBillDTO bill)
        {
            var query = """
                DELETE FROM bill
                WHERE id = @id
                """;
            var queryParams = new List<NpgsqlParameter>()
            {
                new NpgsqlParameter("id", bill.Id),
            };
            var reader = await Helper.UpdateTable(query, queryParams);
            return reader == 1;
        }
    }
}
