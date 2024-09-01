namespace MoneyTracker.Shared.Models.Transaction
{
    public class TransactionDTO
    {
        public int Id { get; set; }
        public string Payee { get; set; }
        public decimal Amount { get; set; }
        public DateTime DatePaid { get; set; }
        public string Category { get; set; }
    }
}
