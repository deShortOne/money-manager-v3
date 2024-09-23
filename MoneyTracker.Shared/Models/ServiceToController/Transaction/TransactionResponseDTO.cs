namespace MoneyTracker.Shared.Models.ServiceToController.Transaction
{
    public class TransactionResponseDTO
    {
        public TransactionResponseDTO(int id, string payee, decimal amount, DateTime datePaid, string category, string accountName)
        {
            Id = id;
            Payee = payee;
            Amount = amount;
            DatePaid = datePaid;
            Category = category;
            AccountName = accountName;
        }

        public int Id { get; private set; }
        public string Payee { get; private set; }
        public decimal Amount { get; private set; }
        public DateTime DatePaid { get; private set; }
        public string Category { get; private set; }
        public string AccountName { get; }

        public override bool Equals(object? obj)
        {
            var other = obj as TransactionResponseDTO;

            if (other == null)
            {
                return false;
            }

            return Id == other.Id && Payee == other.Payee && Amount == other.Amount &&
                DatePaid == other.DatePaid && Category == other.Category &&
                AccountName == other.AccountName;
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}
