using NewRefApp.Models;

namespace NewRefApp.Data.DTOs
{
    public class AccountDetailsViewModel
    {
        public BankDetails BankDetails { get; set; }
        public UpiDetails UpiDetails { get; set; }
        public string TransactionPassword { get; set; }
    }
    public class AccountUpdateDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        // Bank Details
        public string AccountNumber { get; set; }
        public string IFSCCode { get; set; }
        public string AccountName { get; set; }
        public string BankName { get; set; }
        public string BankLocation { get; set; }

        // UPI Details
        public string UpiId { get; set; }

        // Security
        public string TransactionPassword { get; set; }
    }
}
