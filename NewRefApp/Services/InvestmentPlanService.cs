using Microsoft.EntityFrameworkCore;
using NewRefApp.Data;
using NewRefApp.Interfaces;
using NewRefApp.Models;

namespace NewRefApp.Services
{
    public class InvestmentPlanService : IInvestmentPlanService
    {
        private readonly ApplicationDbContext _context;

        public InvestmentPlanService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<InvestmentPlan>> GetAllInvestmentPlansAsync()
        {
            return await _context.InvestmentPlan
                .OrderBy(i => i.CreatedAt)
                .ToListAsync();
        }

        public async Task<InvestmentPlan> GetInvestmentPlanByIdAsync(int id)
        {
            return await _context.InvestmentPlan
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task CreateInvestmentPlanAsync(InvestmentPlan investmentPlan)
        {
            _context.InvestmentPlan.Add(investmentPlan);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateInvestmentPlanAsync(InvestmentPlan investmentPlan)
        {
            _context.InvestmentPlan.Update(investmentPlan);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteInvestmentPlanAsync(int id)
        {
            var investmentPlan = await _context.InvestmentPlan.FindAsync(id);
            if (investmentPlan != null)
            {
                _context.InvestmentPlan.Remove(investmentPlan);
                await _context.SaveChangesAsync();
            }
        }
    }
}
