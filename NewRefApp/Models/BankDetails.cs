using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NewRefApp.Models
{
    public class BankDetails
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(User))]
        public int UserId { get; set; }

        public User User { get; set; }

        [Required]
        [MaxLength(30)]
        public string AccountNumber { get; set; }

        [Required]
        [MaxLength(100)]
        public string AccountName { get; set; }

        [Required]
        [MaxLength(100)]
        public string BankName { get; set; }

        [Required]
        [MaxLength(20)]
        public string IFSCCode { get; set; }

        [MaxLength(150)]
        public string BankLocation { get; set; }

        public bool IsAdmin { get; set; } // e.g., "true - admin", "false - user"

        public bool Status { get; set; } // e.g., "Active", "Inactive"

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? LastUsedAt { get; set; }

    }

}
