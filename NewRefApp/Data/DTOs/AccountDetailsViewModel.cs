using NewRefApp.Models;

namespace NewRefApp.Data.DTOs
{
    public class AccountDetailsViewModel
    {
        public BankDetails BankDetails { get; set; }
        public UpiDetails UpiDetails { get; set; }
        public string TransactionPassword { get; set; } // For Withdraw table
    }
}
