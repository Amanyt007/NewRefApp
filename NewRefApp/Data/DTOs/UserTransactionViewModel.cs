namespace NewRefApp.Data.DTOs
{
    public class UserTransactionViewModel
    {
        public int TransactionId { get; set; }
        public string Type { get; set; } // Deposit, Withdraw, Bonus, Earnings
        public string UserName { get; set; }
        public decimal Amount { get; set; }
        public string PaymentType { get; set; }
        public string Status { get; set; }
        public DateTime Date { get; set; }
        public string DetailsUrl { get; set; } // Link for "View" button
    }

}
