namespace MoneyTracker.Shared.Models.ControllerToService.Transaction
{
    public class EditTransactionRequestDTO
    {
        public EditTransactionRequestDTO(int id, string? payee = null, decimal? amount = null,
            DateTime? datePaid = null, int? category = null, int? accountId = null)
        {
            Id = id;
            Payee = payee;
            Amount = amount;
            DatePaid = datePaid;
            Category = category;
            AccountId = accountId;
        }

        public int Id { get; private set; }
        public string? Payee { get; private set; }
        public decimal? Amount { get; private set; } = null;
        public DateTime? DatePaid { get; private set; } = null;
        public int? Category { get; private set; } = null;
        public int? AccountId { get; }
    }
}
