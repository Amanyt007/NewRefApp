using System.ComponentModel.DataAnnotations.Schema;

namespace NewRefApp.Models
{
    public class UserInvestment
    {
        public int Id { get; set; }
        [ForeignKey(nameof(ReferredByUser))]
        public int UserId { get; set; }
        public User ReferredByUser { get; set; }

        [ForeignKey(nameof(InvestmentPlan))] 
        public int PlanId { get; set; }
        public InvestmentPlan InvestmentPlan { get; set; }
        public DateTime StartDate { get; set; }
        
        public DateTime EndDate { get; set; }
        public int status { get; set; } // "Not purchased - 0" , "purchased - 1","completed - 2"

    }

}
