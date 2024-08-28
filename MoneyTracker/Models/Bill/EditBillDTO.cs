namespace MoneyTracker.API.Models.Bill
{
    public class EditBillDTO
    {
        public int Id { get; set; }
        public string Payee { get; set; }
        public decimal Amount { get; set; }
        public DateTime DatePaid { get; set; }
        public int Category { get; set; }
    }
}
