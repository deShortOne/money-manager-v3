namespace MoneyTracker.Shared.Models.ServiceToRepository.Transaction
{
    public class NewTransactionDTO
    {
        public NewTransactionDTO(string payee, decimal amount, DateOnly datePaid, int category, int accountId)
        {
            Payee = payee;
            Amount = amount;
            DatePaid = datePaid;
            Category = category;
            AccountId = accountId;
        }

        public string Payee { get; private set; }
        public decimal Amount { get; private set; }
        public DateOnly DatePaid { get; private set; }
        public int Category { get; private set; }
        public int AccountId { get; }
    }
}
