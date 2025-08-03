using NewRefApp.Models;

namespace NewRefApp.Interfaces
{
    public interface IInvestmentPlanService
    {
        Task<List<InvestmentPlan>> GetAllInvestmentPlansAsync();
        Task<InvestmentPlan> GetInvestmentPlanByIdAsync(int id);
        Task CreateInvestmentPlanAsync(InvestmentPlan plan);
        Task UpdateInvestmentPlanAsync(InvestmentPlan plan);
        Task DeleteInvestmentPlanAsync(int id);
    }
}
