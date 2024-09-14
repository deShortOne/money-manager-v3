namespace MoneyTracker.Shared.Models.ServiceToRepository.Transaction
{
    public class EditTransactionDTO
    {
        public EditTransactionDTO(int id, string payee, decimal? amount = null,
            DateTime? datePaid = null, int? category = null)
        {
            Id = id;
            Payee = payee;
            Amount = amount;
            DatePaid = datePaid;
            Category = category;
        }

        public int Id { get; private set; }
        public string Payee { get; private set; }
        public decimal? Amount { get; private set; } = null;
        public DateTime? DatePaid { get; private set; } = null;
        public int? Category { get; private set; } = null;
    }
}
