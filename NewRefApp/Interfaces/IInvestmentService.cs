using NewRefApp.Models;

namespace NewRefApp.Interfaces
{
    public interface IInvestmentService
    {
        Task<IEnumerable<InvestmentPlan>> GetAllPlans();
        Task<InvestmentPlan> GetPlanById(int id);
        Task<UserInvestment> PurchasePlan(int userId, int planId);
        Task<IEnumerable<UserInvestment>> GetUserInvestments(int userId);
        Task<bool> CancelInvestment(int investmentId);
    }

}
