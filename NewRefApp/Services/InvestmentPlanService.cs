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

        public async Task<List<InvestmentPlan>> GetAllInvestmentPlansAsync()
        {
            return await _context.InvestmentPlan.ToListAsync();
        }

        public async Task<InvestmentPlan> GetInvestmentPlanByIdAsync(int id)
        {
            return await _context.InvestmentPlan.FindAsync(id);
        }

        public async Task CreateInvestmentPlanAsync(InvestmentPlan plan)
        {
            _context.InvestmentPlan.Add(plan);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateInvestmentPlanAsync(InvestmentPlan plan)
        {
            _context.InvestmentPlan.Update(plan);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteInvestmentPlanAsync(int id)
        {
            var plan = await _context.InvestmentPlan.FindAsync(id);
            if (plan != null)
            {
                _context.InvestmentPlan.Remove(plan);
                await _context.SaveChangesAsync();
            }
        }
    }
}
