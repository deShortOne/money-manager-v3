namespace MoneyTracker.Shared.Models.Transaction
{
    public class NewTransactionDTO
    {
        public NewTransactionDTO(string payee, decimal amount, DateTime datePaid, int category)
        {
            Payee = payee;
            Amount = amount;
            DatePaid = datePaid;
            Category = category;
        }

        public string Payee { get; private set; }
        public decimal Amount { get; private set; }
        public DateTime DatePaid { get; private set; }
        public int Category { get; private set; }
    }
}
