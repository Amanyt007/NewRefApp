using Microsoft.EntityFrameworkCore;
using NewRefApp.Data;
using NewRefApp.Interfaces;
using NewRefApp.Models;

namespace NewRefApp.Services
{
    public class BankDetailsService : IBankDetailsService
    {
        private readonly ApplicationDbContext _context;

        public BankDetailsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<BankDetails> GetBankDetailByIdAsync(int id)
        {
            return await _context.BankDetails
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<IEnumerable<BankDetails>> GetAllBankDetailsAsync()
        {
            return await _context.BankDetails
                .Include(b => b.User)
                .ToListAsync();
        }

        public async Task CreateBankDetailAsync(BankDetails bankDetails)
        {
            _context.BankDetails.Add(bankDetails);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateBankDetailAsync(BankDetails bankDetails)
        {
            _context.BankDetails.Update(bankDetails);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteBankDetailAsync(int id)
        {
            var bankDetail = await _context.BankDetails.FindAsync(id);
            if (bankDetail != null)
            {
                _context.BankDetails.Remove(bankDetail);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<BankDetails> GetFirstActiveAdminBankAsync()
        {
            return await _context.BankDetails
                .Where(b => b.IsAdmin && b.Status)
                .OrderBy(b => b.Id)
                .FirstOrDefaultAsync();
        }
    }
}
