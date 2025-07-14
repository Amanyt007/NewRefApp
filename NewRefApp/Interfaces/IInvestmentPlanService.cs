using NewRefApp.Models;

namespace NewRefApp.Interfaces
{
    public interface IInvestmentPlanService
    {
        Task<IEnumerable<InvestmentPlan>> GetAllInvestmentPlansAsync();
        Task<InvestmentPlan> GetInvestmentPlanByIdAsync(int id);
        Task CreateInvestmentPlanAsync(InvestmentPlan investmentPlan);
        Task UpdateInvestmentPlanAsync(InvestmentPlan investmentPlan);
        Task DeleteInvestmentPlanAsync(int id);
    }
}
