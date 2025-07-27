namespace NewRefApp.Data.DTOs
{
    public class TransactionViewModel
    {
        public string TransactionType { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string DetailsLink { get; set; } // For the "View" button
    }

    public class UserTransactionsViewModel
    {
        public List<TransactionViewModel> Transactions { get; set; }
        public decimal UserBalance { get; set; }
    }
}
