namespace MoneyTracker.Shared.Models.Transaction
{
    public class TransactionDTO
    {
        public TransactionDTO(int id, string payee, decimal amount, DateTime datePaid, string category)
        {
            Id = id;
            Payee = payee;
            Amount = amount;
            DatePaid = datePaid;
            Category = category;
        }

        public int Id { get; private set; }
        public string Payee { get; private set; }
        public decimal Amount { get; private set; }
        public DateTime DatePaid { get; private set; }
        public string Category { get; private set; }
    }
}
