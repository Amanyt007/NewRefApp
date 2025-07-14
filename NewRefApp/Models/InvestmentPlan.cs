using System.ComponentModel.DataAnnotations;

namespace NewRefApp.Models
{
    public class InvestmentPlan
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        public string Category { get; set; }

        [Required]
        public string RevenueDurationValue { get; set; }

        [Required]
        public int? VipLevel { get; set; }

        public decimal? DailyEarningsPerUnit { get; set; }

        public decimal? HourlyEarningsPerUnit { get; set; }

        [Required]
        public decimal InvestmentAmount { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

}
