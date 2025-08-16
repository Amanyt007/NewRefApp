namespace NewRefApp.Data.DTOs
{
    public class WithdrawDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public decimal Amount { get; set; }
        public int Status { get; set; }
        public int PaymentType { get; set; }
        public string UpiId { get; set; }
        public string BankAccount { get; set; }
        public DateTime Date { get; set; }
    }

}
