namespace MoneyTracker.Shared.Models.Transaction
{
    public class EditTransactionDTO
    {
        public int Id { get; set; }
        public string Payee { get; set; } = null;
        public decimal? Amount { get; set; } = null;
        public DateTime? DatePaid { get; set; } = null;
        public int? Category { get; set; } = null;
    }
}
