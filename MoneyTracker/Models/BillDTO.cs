namespace MoneyTracker.API.Models
{
    public class BillDTO
    {
        public int Id { get; set; }
        public string Payee { get; set; }
        public decimal Amount { get; set; }
        public DateTime DatePaid { get; set; }
        public string Category { get; set; }
    }
}
