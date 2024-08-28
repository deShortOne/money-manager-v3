namespace MoneyTracker.API.Models.Bill
{
    public class NewBillDTO
    {
        public string Payee { get; set; }
        public decimal Amount { get; set; }
        public DateTime DatePaid { get; set; }
        public int Category { get; set; }
    }
}
