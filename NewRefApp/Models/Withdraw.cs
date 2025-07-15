using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NewRefApp.Models
{
    public class Withdraw
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(User))]
        public int UserId { get; set; }

        public User User { get; set; }

        public decimal Amount { get; set; }

        public int Status { get; set; }  // e.g., "Pending - 0", "Completed - 1", "Failed - 2"

        public int PaymentType { get; set; } // e.g., "UPI - 0", "BankTransfer - 1"

        [ForeignKey(nameof(BankDetail))]
        public int? BankDetailId { get; set; }

        [ForeignKey(nameof(UpiDetail))]
        public int? UpiDetailId { get; set; }

        public string transactionPassword { get; set; } // as per user's registration

        public BankDetails BankDetail { get; set; }

        public UpiDetails UpiDetail { get; set; }

        public DateTime Date { get; set; } = DateTime.UtcNow;
    }
}
