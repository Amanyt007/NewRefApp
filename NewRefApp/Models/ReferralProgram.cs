using System.ComponentModel.DataAnnotations;

namespace NewRefApp.Models
{
    public class ReferralProgram
    {
        [Key]
        public int Id { get; set; }

        public int Level { get; set; }
        public float Percent { get; set; }
    }

}
