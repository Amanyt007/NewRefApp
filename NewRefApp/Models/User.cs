using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NewRefApp.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string WithdrawnPassword { get; set; }
        public string ReferralCode { get; set; }

        public int? ReferredBy { get; set; }

        public int VipLevel { get; set; }
        public bool IsPurchased { get; set; }
        public bool IsActive { get; set; }
        public bool IsAdmin { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }


}
