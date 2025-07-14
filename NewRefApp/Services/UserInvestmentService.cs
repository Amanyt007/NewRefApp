using Microsoft.EntityFrameworkCore;
using NewRefApp.Data;
using NewRefApp.Interfaces;
using NewRefApp.Models;

namespace NewRefApp.Services
{
    public class UserInvestmentService : IUserInvestmentService
    {
        private readonly ApplicationDbContext _context;

        public UserInvestmentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserInvestment>> GetAllUserInvestmentsAsync()
        {
            return await _context.UserInvestment
                .Include(ui => ui.ReferredByUser)
                .Include(ui => ui.InvestmentPlan)
                .OrderByDescending(ui => ui.StartDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<UserInvestment>> GetUserInvestmentsByUserIdAsync(int userId)
        {
            return await _context.UserInvestment
                .Include(ui => ui.ReferredByUser)
                .Include(ui => ui.InvestmentPlan)
                .Where(ui => ui.UserId == userId)
                .OrderByDescending(ui => ui.StartDate)
                .ToListAsync();
        }

        public async Task<UserInvestment> GetUserInvestmentByIdAsync(int id)
        {
            return await _context.UserInvestment
                .Include(ui => ui.ReferredByUser)
                .Include(ui => ui.InvestmentPlan)
                .FirstOrDefaultAsync(ui => ui.Id == id);
        }

        public async Task CreateUserInvestmentAsync(UserInvestment userInvestment)
        {
            _context.UserInvestment.Add(userInvestment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserInvestmentAsync(UserInvestment userInvestment)
        {
            _context.UserInvestment.Update(userInvestment);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUserInvestmentAsync(int id)
        {
            var userInvestment = await _context.UserInvestment.FindAsync(id);
            if (userInvestment != null)
            {
                _context.UserInvestment.Remove(userInvestment);
                await _context.SaveChangesAsync();
            }
        }
    }
}
