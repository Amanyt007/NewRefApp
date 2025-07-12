using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NewRefApp.Models
{
    public class ActivityLog
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(User))]
        public int UserId { get; set; }

        public User User { get; set; }

        public DateTime LastLogin { get; set; }
        public DateTime LastLogout { get; set; }
    }
}
