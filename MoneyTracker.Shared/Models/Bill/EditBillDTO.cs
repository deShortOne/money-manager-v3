namespace MoneyTracker.Shared.Models.Bill
{
    public class EditBillDTO
    {
        public int Id { get; set; }
        public string Payee { get; set; } = null;
        public decimal? Amount { get; set; } = null;
        public DateTime? DatePaid { get; set; } = null;
        public int? Category { get; set; } = null;
    }
}
