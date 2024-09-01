namespace MoneyTracker.Shared.Models.Transaction
{
    public class NewTransactionDTO
    {
        public string Payee { get; set; }
        public decimal Amount { get; set; }
        public DateTime DatePaid { get; set; }
        public int Category { get; set; }
    }
}
