
namespace MoneyTracker.API.Database
{
    public class Budget
    {
        public async void GetBudget()
        {
            using (var reader = await Helper.GetTable("SELECT 1"))
            {
                while (await reader.ReadAsync())
                Console.WriteLine(reader.GetString(0));
            }
        }
    }
}
