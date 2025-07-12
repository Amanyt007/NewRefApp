using Microsoft.EntityFrameworkCore;
using NewRefApp.Data;
using NewRefApp.Interfaces;
using NewRefApp.Models;

namespace NewRefApp.Services
{
    public class DepositService : IDepositService
    {
        private readonly ApplicationDbContext _context;

        public DepositService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Deposit> GetDepositByIdAsync(int id)
        {
            return await _context.Deposit
                .Include(d => d.User)
                .Include(d => d.BankDetail)
                .Include(d => d.UpiDetail)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<IEnumerable<Deposit>> GetAllDepositsAsync()
        {
            return await _context.Deposit
                .Include(d => d.User)
                .Include(d => d.BankDetail)
                .Include(d => d.UpiDetail)
                .ToListAsync();
        }

        public async Task CreateDepositAsync(Deposit deposit)
        {
            _context.Deposit.Add(deposit);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateDepositAsync(Deposit deposit)
        {
            _context.Deposit.Update(deposit);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteDepositAsync(int id)
        {
            var deposit = await _context.Deposit.FindAsync(id);
            if (deposit != null)
            {
                _context.Deposit.Remove(deposit);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<User> GetUserByPhoneAsync(string phoneNumber)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
        }
    }
}
