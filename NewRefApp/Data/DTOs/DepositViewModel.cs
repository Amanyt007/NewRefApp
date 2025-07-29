namespace NewRefApp.Data.DTOs
{
    public class DepositViewModel
    {
        public int Id { get; set; }

        public string UserFullName { get; set; }

        public decimal Amount { get; set; }

        public int PaymentType { get; set; }

        public string PaymentTypeLabel => PaymentType == 0 ? "UPI" : "BankTransfer";

        public string UtrNumber { get; set; }

        public DateTime Date { get; set; }

        public int Status { get; set; } // e.g., "Pending - 0", "Completed - 1", "Failed - 2"
    }

}
