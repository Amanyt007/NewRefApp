using Microsoft.EntityFrameworkCore;
using NewRefApp.Data;
using NewRefApp.Interfaces;
using NewRefApp.Models;

namespace NewRefApp.Services
{
    public class TransactionDetailsService : ITransactionDetailsService
    {
        private readonly ApplicationDbContext _context;

        public TransactionDetailsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Deposit>> GetUserTransactionsAsync(string userPhone)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.PhoneNumber == userPhone);
            if (user == null) return new List<Deposit>();

            return await _context.Deposit
                .Include(d => d.User)
                .Include(d => d.UpiDetail)
                .Include(d => d.BankDetail)
                .Where(d => d.UserId == user.Id)
                .OrderByDescending(d => d.Date)
                .ToListAsync();
        }

        public async Task<Deposit> GetTransactionByIdAsync(int id)
        {
            return await _context.Deposit
                .Include(d => d.User)
                .Include(d => d.UpiDetail)
                .Include(d => d.BankDetail)
                .FirstOrDefaultAsync(d => d.Id == id);
        }
    }
}
