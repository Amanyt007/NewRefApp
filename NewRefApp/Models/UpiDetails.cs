using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NewRefApp.Models
{
    public class UpiDetails
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(User))]
        public int UserId { get; set; }

        public User User { get; set; }

        [Required]
        [MaxLength(30)]
        public string UpiId { get; set; }

        public bool IsAdmin { get; set; } // e.g., "true - admin", "false - user"

        public bool Status { get; set; } // e.g., "Active", "Inactive"

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
