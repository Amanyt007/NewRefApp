using Microsoft.EntityFrameworkCore;
using NewRefApp.Data;
using NewRefApp.Interfaces;
using NewRefApp.Models;

namespace NewRefApp.Services
{
    public class WithdrawService : IWithdrawService
    {
        private readonly ApplicationDbContext _context;

        public WithdrawService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Withdraw>> GetAllWithdrawsAsync()
        {
            return await _context.Withdraw
                .Include(w => w.User)
                .Include(w => w.BankDetail)
                .OrderByDescending(w => w.Date)
                .ToListAsync();
        }

        public async Task<Withdraw> GetWithdrawByIdAsync(int id)
        {
            return await _context.Withdraw
                .Include(w => w.User)
                .Include(w => w.BankDetail)
                .FirstOrDefaultAsync(w => w.Id == id);
        }

        public async Task<Withdraw> GetWithdrawByUserIdAsync(int userId)
        {
            return await _context.Withdraw
                .Include(w => w.User)
                .Include(w => w.BankDetail)
                .FirstOrDefaultAsync(w => w.UserId == userId);
        }

        public async Task<IEnumerable<Withdraw>> GetWithdrawalsByUserIdAsync(int userId)
        {
            return await _context.Withdraw
                .Include(w => w.User)
                .Include(w => w.BankDetail)
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.Date)
                .ToListAsync();
        }

        public async Task CreateWithdrawAsync(Withdraw withdraw)
        {
            _context.Withdraw.Add(withdraw);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateWithdrawAsync(Withdraw withdraw)
        {
            _context.Withdraw.Update(withdraw);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteWithdrawAsync(int id)
        {
            var withdraw = await _context.Withdraw.FindAsync(id);
            if (withdraw != null)
            {
                _context.Withdraw.Remove(withdraw);
                await _context.SaveChangesAsync();
            }
        }
    }
}
